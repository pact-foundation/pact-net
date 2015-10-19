using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Models;
using PactNet.Reporters;
using PactNet.Schemas.Interfaces;
using PactNet.Schemas.Models;
using PactNet.Schemas.Validators;

namespace PactNet.Schemas.Verifiers
{
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
    [SuppressMessage("ReSharper", "UseNullPropagation")]
    class PactSchemaVerifier : IPactSchemaVerifier
    {
        private readonly IFileSystem _fileSystem;
        private readonly Func<IReporter, PactVerifierConfig, IProviderDataSchemaValidator> _providerServiceValidatorFactory;
        private readonly PactVerifierConfig _config;
        private HttpClient _httpClient;

        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }
        public JObject Document { get; private set; }
        public ProviderStates ProviderStates { get; private set; }
        public string PactFileUri { get; private set; }
        public PactUriOptions PactUriOptions { get; private set; }

        internal PactSchemaVerifier(
            Action setUp,
            Action tearDown,
            IFileSystem fileSystem,
            Func<IReporter, PactVerifierConfig, IProviderDataSchemaValidator> providerServiceValidatorFactory,
            PactVerifierConfig config,
            HttpClient httpClient = null)
        {
            _fileSystem = fileSystem;
            _providerServiceValidatorFactory = providerServiceValidatorFactory;
            _config = config ?? new PactVerifierConfig();
            _httpClient = httpClient ?? new HttpClient();

            ProviderStates = new ProviderStates(setUp, tearDown);
        }

        /// <summary>
        /// Define any set up and tear down state that is required when running the interaction verify.
        /// We strongly recommend that any set up state is cleared using the tear down. This includes any state and IoC container overrides you may be doing.
        /// </summary>
        /// <param name="setUp">A set up action that will be run before each interaction verify. If no action is required please use an empty lambda () => {}.</param>
        /// <param name="tearDown">A tear down action that will be run after each interaction verify. If no action is required please use an empty lambda () => {}.</param>
        /// <param name="config"></param>
        public PactSchemaVerifier(Action setUp, Action tearDown, PactVerifierConfig config = null)
            : this(
            setUp, 
            tearDown,
            new FileSystem(), 
            (reporter, verifierConfig) => new ProviderDataSchemaValidator(reporter, verifierConfig),  config)
        {
        }

        public IPactSchemaVerifier ProviderState(string providerState, Action setUp = null, Action tearDown = null)
        {
            if (string.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            var providerStateItem = new ProviderState(providerState, setUp, tearDown);
            ProviderStates.Add(providerStateItem);

            return this;
        }

        public IPactSchemaVerifier HonoursPactWith(string consumerName)
        {
            if (string.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            if (!string.IsNullOrEmpty(ConsumerName))
            {
                throw new ArgumentException("ConsumerName has already been supplied, please instantiate a new PactVerifier if you want to perform verification for a different consumer");
            }

            ConsumerName = consumerName;

            return this;
        }

        public IPactSchemaVerifier PactUri(string uri, PactUriOptions options = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            PactFileUri = uri;
            PactUriOptions = options;

            return this;
        }

        public IPactSchemaVerifier DocumentToValidate(JObject document)
        {
            if (document == null)
            {
                throw new ArgumentException("Please supply a non null or empty JSON document to valudate");
            }

            if (Document != null)
            {
                throw new ArgumentException("Document has already been supplied, please instantiate a new PactVerifier if you want to perform verification for a different document");
            }

            Document = document;

            return this;
        }

        public void Verify(string description = null, string providerState = null)
        {
            if (string.IsNullOrEmpty(PactFileUri))
            {
                throw new InvalidOperationException("PactFileUri has not been set, please supply a uri using the PactUri method.");
            }

            ProviderSchemaPactFile pactFile;
            try
            {
                string pactFileJson;

                if (IsWebUri(PactFileUri))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, PactFileUri);
                    request.Headers.Add("Accept", "application/json");

                    if (PactUriOptions != null)
                    {
                        request.Headers.Add("Authorization", string.Format("{0} {1}", PactUriOptions.AuthorizationScheme, PactUriOptions.AuthorizationValue));
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
                else 
                {
                    pactFileJson = _fileSystem.File.ReadAllText(PactFileUri);
                }

                pactFile = JsonConvert.DeserializeObject<ProviderSchemaPactFile>(pactFileJson);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Json Pact file could not be retrieved using uri \'{0}\'.", PactFileUri), ex);
            }

            // Filter interactions
            if (description != null)
            {
                pactFile.Schemas = pactFile.Schemas.Where(x => x.Description.Equals(description));
            }

            if (providerState != null)
            {
                pactFile.Schemas = pactFile.Schemas.Where(x => x.ProviderState.Equals(providerState));
            }

            if ((description != null || providerState != null) && (pactFile.Schemas == null || !pactFile.Schemas.Any()))
            {
                throw new ArgumentException("The specified description and/or providerState filter yielded no interactions.");
            }

            var loggerName = LogProvider.CurrentLogProvider.AddLogger(_config.LogDir, ProviderName.ToLowerSnakeCase(), "{0}_verifier.log");
            _config.LoggerName = loggerName;

            try
            {
                _providerServiceValidatorFactory(new Reporter(_config), _config).Validate(pactFile, ProviderStates, Document);
            }
            finally
            {
                LogProvider.CurrentLogProvider.RemoveLogger(_config.LoggerName);
            }
        }
        
        private static bool IsWebUri(string uri)
        {
            return uri.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || uri.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase);
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
