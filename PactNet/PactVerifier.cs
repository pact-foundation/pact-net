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
    public class PactVerifier : IPactVerifier, IProviderStates
    {
        private readonly IFileSystem _fileSystem;
        private readonly Func<IHttpRequestSender, IProviderServiceValidator> _providerServiceValidatorFactory;
        private readonly HttpClient _httpClient;
        private IHttpRequestSender _httpRequestSender;

        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }
        public ProviderStates ProviderStates { get; private set; }
        public string PactFileUri { get; private set; }

        internal PactVerifier(IFileSystem fileSystem,
            Func<IHttpRequestSender, IProviderServiceValidator> providerServiceValidatorFactory, 
            HttpClient httpClient)
        {
            _fileSystem = fileSystem;
            _providerServiceValidatorFactory = providerServiceValidatorFactory;
            _httpClient = httpClient;
        }

        public PactVerifier() : this(
            new FileSystem(),
            httpRequestSender => new ProviderServiceValidator(httpRequestSender, new Reporter()),
            new HttpClient())
        {
        }

        public IProviderStates ProviderStatesFor(string consumerName, Action setUp = null, Action tearDown = null)
        {
            if (String.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            if (!String.IsNullOrEmpty(ConsumerName) && !ConsumerName.Equals(consumerName))
            {
                throw new ArgumentException("Please supply the same consumerName that was defined when calling the HonoursPactWith method");
            }

            ConsumerName = consumerName;
            ProviderStates = new ProviderStates(setUp, tearDown);

            return this;
        }

        public IProviderStates ProviderState(string providerState, Action setUp = null, Action tearDown = null)
        {
            if (ProviderStates == null)
            {
                throw new InvalidOperationException("Please intitialise the provider states by first calling the ProviderStatesFor method");
            }

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

            if (!String.IsNullOrEmpty(ConsumerName) && !ConsumerName.Equals(consumerName))
            {
                throw new ArgumentException("Please supply the same consumerName that was defined when calling the ProviderStatesFor method");
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

        public void Verify(string providerDescription = null, string providerState = null)
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
                        Dipose(request);
                        Dipose(response);
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
            if (providerDescription != null)
            {
                pactFile.Interactions = pactFile.Interactions.Where(x => x.Description.Equals(providerDescription));
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

        private static void Dipose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
