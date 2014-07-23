using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Mocks.MockHttpService.Validators;

namespace PactNet
{
    public partial class Pact : IPactProvider
    {
        private readonly Func<HttpClient, IProviderServiceValidator> _providerServiceValidatorFactory;
        private Dictionary<string, Action> _providerStates;
        public IReadOnlyDictionary<string, Action> ProviderStates { get { return _providerStates; } }

        public HttpClient HttpClient { get; private set; }

        public IPactProvider ProviderStatesFor(string consumerName, Dictionary<string, Action> providerStates)
        {
            if (String.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            if (providerStates == null || !providerStates.Any())
            {
                throw new ArgumentException("Please supply a non null or empty dictionary of providerStates");
            }

            if (!String.IsNullOrEmpty(ConsumerName) && !ConsumerName.Equals(consumerName))
            {
                throw new ArgumentException("Please supply the same consumerName that was defined when calling the HonoursPactWith method");
            }

            ConsumerName = consumerName;
            _providerStates = providerStates;

            return this;
        }

        public IPactProvider ServiceProvider(string providerName, HttpClient httpClient)
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
            HttpClient = httpClient;

            return this;
        }

        public IPactProvider HonoursPactWith(string consumerName)
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

        public IPactProvider PactUri(string uri)
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
            if (HttpClient == null)
            {
                throw new InvalidOperationException("HttpClient has not been set, please supply a HttpClient using the ServiceProvider method.");
            }

            if (String.IsNullOrEmpty(PactFileUri) || String.IsNullOrEmpty(_pactFileUri))
            {
                throw new InvalidOperationException("PactFileUri has not been set, please supply a uri using the PactUri method.");
            }

            ServicePactFile pactFile;
            try
            {
                var pactFileJson = _fileSystem.File.ReadAllText(PactFileUri);
                pactFile = JsonConvert.DeserializeObject<ServicePactFile>(pactFileJson);
            }
            catch (System.IO.IOException)
            {
                throw new CompareFailedException(String.Format("Json Pact file could not be retrieved using uri \'{0}\'.", PactFileUri));
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

            //Invoke provide state on interactions
            if (pactFile.Interactions != null && pactFile.Interactions.Any(x => x.ProviderState != null))
            {
                foreach (var interactionProviderState in pactFile.Interactions.Where(x => x.ProviderState != null).Select(x => x.ProviderState))
                {
                    if (_providerStates == null || !_providerStates.Any() || !_providerStates.ContainsKey(interactionProviderState))
                    {
                        throw new InvalidOperationException(String.Format("providerState \"{0}\" could not be found, please supply the provider state using the ProviderStatesFor method.", interactionProviderState));
                    }
                    _providerStates[interactionProviderState].Invoke();
                }
            }

            _providerServiceValidatorFactory(HttpClient).Validate(pactFile);
        }
    }
}
