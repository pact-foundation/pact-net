using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Provider;

namespace PactNet
{
    public partial class Pact : IPactProvider
    {
        private IDictionary<string, Action> _providerStates;
        private string _pactUri;

        private HttpClient _httpClient;

        public IPactProvider ProviderStatesFor(string consumerName, IDictionary<string, Action> providerStates)
        {
            ConsumerName = consumerName;
            _providerStates = providerStates;

            return this;
        }

        public IPactProvider ServiceProvider(string providerName, HttpClient httpClient)
        {
            ProviderName = providerName;
            _httpClient = httpClient;

            return this;
        }

        public IPactProvider HonoursPactWith(string consumerName)
        {
            ConsumerName = consumerName;

            return this;
        }

        public IPactProvider PactUri(string uri)
        {
            _pactUri = uri;

            return this;
        }

        public void Execute()
        {
            if (_httpClient == null)
            {
                throw new InvalidOperationException("httpClient has not been set, please supply a HttpClient using the ServiceProvider method.");
            }

            PactFile pactFile;
            try
            {
                var pactFileJson = _fileSystem.File.ReadAllText(_pactUri);
                pactFile = JsonConvert.DeserializeObject<PactFile>(pactFileJson, JsonConfig.SerializerSettings);
            }
            catch (System.IO.IOException)
            {
                throw new PactAssertException(String.Format("Json Pact file could not be retrieved using uri \'{0}\'.", _pactUri));
            }

            if (pactFile.Interactions != null && pactFile.Interactions.Any())
            {
                foreach (var providerState in pactFile.Interactions.Where(x => x.ProviderState != null).Select(x => x.ProviderState))
                {
                    if (_providerStates == null || !_providerStates.Any() || !_providerStates.ContainsKey(providerState))
                    {
                        throw new PactAssertException(String.Format("No provider state has been supplied for \"{0}\" as defined in the json Pact file. Please use ProviderStatesFor method when defining the Provider Pact.", providerState));
                    }
                    _providerStates[providerState].Invoke();
                }
            }

            pactFile.VerifyProvider(_httpClient);
        }
    }
}
