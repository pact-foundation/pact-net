using System;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;
using PactNet.Models;
using PactNet.Reporters;
using System.Text;

namespace PactNet
{
    public class PactVerifier : IPactVerifier
    {
        private readonly IFileSystem _fileSystem;
        private readonly Func<IHttpRequestSender, IReporter, PactVerifierConfig, IProviderServiceValidator> _providerServiceValidatorFactory;
        private readonly HttpClient _httpClient;
        private readonly PactVerifierConfig _config;

        private IHttpRequestSender _httpRequestSender;

        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }
        public ProviderStates ProviderStates { get; private set; }
        public string PactFileUri { get; private set; }
        public PactUriOptions PactUriOptions { get; private set; }

        internal PactVerifier(
            Action setUp, 
            Action tearDown,
            IFileSystem fileSystem,
            Func<IHttpRequestSender, IReporter, PactVerifierConfig, IProviderServiceValidator> providerServiceValidatorFactory, 
            HttpClient httpClient,
            PactVerifierConfig config)
        {
            _fileSystem = fileSystem;
            _providerServiceValidatorFactory = providerServiceValidatorFactory;
            _httpClient = httpClient;
            _config = config ?? new PactVerifierConfig();

            ProviderStates = new ProviderStates(setUp, tearDown);
        }

        /// <summary>
        /// Define any set up and tear down state that is required when running the interaction verify.
        /// We strongly recommend that any set up state is cleared using the tear down. This includes any state and IoC container overrides you may be doing.
        /// </summary>
        /// <param name="setUp">A set up action that will be run before each interaction verify. If no action is required please use an empty lambda () => {}.</param>
        /// <param name="tearDown">A tear down action that will be run after each interaction verify. If no action is required please use an empty lambda () => {}.</param>
        /// <param name="config"></param>
        public PactVerifier(Action setUp, Action tearDown, PactVerifierConfig config = null)
            : this(
            setUp, 
            tearDown,
            new FileSystem(),
            (httpRequestSender, reporter, verifierConfig) => new ProviderServiceValidator(httpRequestSender, reporter, verifierConfig),
            new HttpClient(),
            config)
        {
        }

        /// <summary>
        /// Define a set up and/or tear down action for a specific state specified by the consumer.
        /// This is where you should set up test data, so that you can fulfil the contract outlined by a consumer.
        /// </summary>
        /// <param name="providerState">The name of the provider state as defined by the consumer interaction, which lives in the Pact file.</param>
        /// <param name="setUp">A set up action that will be run before the interaction verify, if the provider has specified it in the interaction. If no action is required please use an empty lambda () => {}.</param>
        /// <param name="tearDown">A tear down action that will be run after the interaction verify, if the provider has specified it in the interaction. If no action is required please use an empty lambda () => {}.</param>
        public IPactVerifier ProviderState(string providerState, Action setUp = null, Action tearDown = null)
        {
            if (String.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            var providerStateItem = new ProviderState(providerState, setUp, tearDown);
            ProviderStates.Add(providerStateItem);

            return this;
        }

        public IPactVerifier ServiceProvider(string providerName, HttpClient httpClient)
        {
            if (String.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            if (!String.IsNullOrEmpty(ProviderName))
            {
                throw new ArgumentException("ProviderName has already been supplied, please instantiate a new PactVerifier if you want to perform verification for a different provider");
            }

            if (httpClient == null)
            {
                throw new ArgumentException("Please supply a non null httpClient");
            }

            ProviderName = providerName;
            _httpRequestSender = new HttpClientRequestSender(httpClient);
                
            return this;
        }

        public IPactVerifier ServiceProvider(string providerName, Func<ProviderServiceRequest, ProviderServiceResponse> httpRequestSender)
        {
            if (String.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            if (!String.IsNullOrEmpty(ProviderName))
            {
                throw new ArgumentException("ProviderName has already been supplied, please instantiate a new PactVerifier if you want to perform verification for a different provider");
            }

            if (httpRequestSender == null)
            {
                throw new ArgumentException("Please supply a non null httpRequestSenderFunc");
            }

            ProviderName = providerName;
            _httpRequestSender = new CustomRequestSender(httpRequestSender);

            return this;
        }

        public IPactVerifier HonoursPactWith(string consumerName)
        {
            if (String.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            if (!String.IsNullOrEmpty(ConsumerName))
            {
                throw new ArgumentException("ConsumerName has already been supplied, please instantiate a new PactVerifier if you want to perform verification for a different consumer");
            }

            ConsumerName = consumerName;

            return this;
        }

        public IPactVerifier PactUri(string uri, PactUriOptions options = null)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            PactFileUri = uri;
            PactUriOptions = options;

            return this;
        }

        public void Verify(string description = null, string providerState = null)
        {
            if (_httpRequestSender == null)
            {
                throw new InvalidOperationException(
                    "httpRequestSender has not been set, please supply a httpClient or httpRequestSenderFunc using the ServiceProvider method.");
            }

            if (String.IsNullOrEmpty(PactFileUri))
            {
                throw new InvalidOperationException(
                    "PactFileUri has not been set, please supply a uri using the PactUri method.");
            }

            ProviderServicePactFile pactFile;
            try
            {
                string pactFileJson;

                if (IsWebUri(PactFileUri))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, PactFileUri);
                    request.Headers.Add("Accept", "application/json");

                    if (PactUriOptions != null)
                    {
                        request.Headers.Add("Authorization", String.Format("{0} {1}", PactUriOptions.AuthorizationScheme, PactUriOptions.AuthorizationValue));
                    }

                    var response = _httpClient.SendAsync(request).Result;

                    try
                    {
                        response.EnsureSuccessStatusCode();
                        pactFileJson = response.Content.ReadAsStringAsync().Result;
                    }
                    finally
                    {
                        Dispose(request);
                        Dispose(response);
                    }
                }
                else //Assume it's a file uri, and we will just throw if it does not exist
                {
                    pactFileJson = _fileSystem.File.ReadAllText(PactFileUri);
                }

                pactFile = JsonConvert.DeserializeObject<ProviderServicePactFile>(pactFileJson);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Json Pact file could not be retrieved using uri \'{0}\'.", PactFileUri), ex);
            }

            //Filter interactions
            if (description != null)
            {
                pactFile.Interactions = pactFile.Interactions.Where(x => x.Description.Equals(description));
            }

            if (providerState != null)
            {
                pactFile.Interactions = pactFile.Interactions.Where(x => x.ProviderState.Equals(providerState));
            }

            if ((description != null || providerState != null) &&
                (pactFile.Interactions == null || !pactFile.Interactions.Any()))
            {
                throw new ArgumentException("The specified description and/or providerState filter yielded no interactions.");
            }

            var loggerName = LogProvider.CurrentLogProvider.AddLogger(_config.LogDir, ProviderName.ToLowerSnakeCase(), "{0}_verifier.log");
            _config.LoggerName = loggerName;

            try
            {
                _providerServiceValidatorFactory(_httpRequestSender, new Reporter(_config), _config)
                    .Validate(pactFile, ProviderStates);
            }
            finally
            {
                LogProvider.CurrentLogProvider.RemoveLogger(_config.LoggerName);
            }
        }

        private static bool IsWebUri(string uri)
        {
            return uri.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                   uri.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase);
        }

        private static void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
