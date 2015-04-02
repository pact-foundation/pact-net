using System;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;
using PactNet.Models;
using PactNet.Reporters;

namespace PactNet
{
    public class PactVerifier : IPactVerifier
    {
        private readonly IFileSystem _fileSystem;
        private readonly Func<IHttpRequestSender, IProviderServiceValidator> _providerServiceValidatorFactory;
        private readonly HttpClient _httpClient;
        private IHttpRequestSender _httpRequestSender;

        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }
        public ProviderStates ProviderStates { get; private set; }
        public string PactFileUri { get; private set; }

        internal PactVerifier(
            Action setUp, 
            Action tearDown,
            IFileSystem fileSystem,
            Func<IHttpRequestSender, IProviderServiceValidator> providerServiceValidatorFactory, 
            HttpClient httpClient)
        {
            _fileSystem = fileSystem;
            _providerServiceValidatorFactory = providerServiceValidatorFactory;
            _httpClient = httpClient;

            ProviderStates = new ProviderStates(setUp, tearDown);
        }

        /// <summary>
        /// Define any set up and tear down state that is required when running the interaction verify.
        /// We strongly recommend that any set up state is cleared using the tear down. This includes any state and IoC container overrides you may be doing.
        /// </summary>
        /// <param name="consumerName">The name of the consumer being verified.</param>
        /// <param name="setUp">A set up action that will be run before each interaction verify. If no action is required please use an empty lambda () => {}.</param>
        /// <param name="tearDown">A tear down action that will be run after each interaction verify. If no action is required please use an empty lambda () => {}.</param>
        public PactVerifier(Action setUp, Action tearDown) : this(
            setUp, 
            tearDown,
            new FileSystem(),
            httpRequestSender => new ProviderServiceValidator(httpRequestSender, new Reporter()),
            new HttpClient())
        {
        }

        [Obsolete("Please supply this information in the constructor. Will be removed in the next major version.")]
        public IPactVerifier ProviderStatesFor(string consumerName, Action setUp = null, Action tearDown = null)
        {
            ProviderStates = new ProviderStates(setUp, tearDown);

            return this;
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

            ConsumerName = consumerName;

            return this;
        }

        public IPactVerifier PactUri(string uri)
        {
            if (String.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            PactFileUri = uri;

            return this;
        }

        public void Verify(string description = null, string providerState = null)
        {
            if (_httpRequestSender == null)
            {
                throw new InvalidOperationException("httpRequestSender has not been set, please supply a httpClient or httpRequestSenderFunc using the ServiceProvider method.");
            }

            if (String.IsNullOrEmpty(PactFileUri))
            {
                throw new InvalidOperationException("PactFileUri has not been set, please supply a uri using the PactUri method.");
            }

            ProviderServicePactFile pactFile;
            try
            {
                string pactFileJson;

                if (IsWebUri(PactFileUri))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, PactFileUri);
                    request.Headers.Add("Accept", "application/json");

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

            _providerServiceValidatorFactory(_httpRequestSender).Validate(pactFile, ProviderStates);
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
