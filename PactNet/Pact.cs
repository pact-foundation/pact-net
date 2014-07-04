using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Consumer;
using PactNet.Consumer.Mocks.MockService;
using PactNet.Provider;

namespace PactNet
{
    //TODO: Implement a Pact file broker
    //TODO: Allow specification of a Pact file path

    public class Pact : IPactConsumer, IPactProvider
    {
        private string _consumerName;
        private string _providerName;
        private IDictionary<string, Action> _providerStates;
        private string _pactUri;

        private IMockProviderService _mockProviderService;
        private const string PactFileDirectory = "../../pacts/";
        private HttpClient _httpClient;
        

        private string PactFilePath
        {
            get { return Path.Combine(PactFileDirectory, PactFileName); }
        }

        private string PactFileName
        {
            get { return String.Format("{0}-{1}.json", _consumerName, _providerName).Replace(' ', '_').ToLower(); }
        }

        public IPactConsumer ServiceConsumer(string consumerName)
        {
            _consumerName = consumerName;

            return this;
        }

        public IPactConsumer HasPactWith(string providerName)
        {
            _providerName = providerName;

            return this;
        }

        public IMockProviderService MockService(int port)
        {
            _mockProviderService = new MockProviderService(port);

            _mockProviderService.Start();

            return _mockProviderService;
        }

        public IPactProvider ProviderStatesFor(string consumerName, IDictionary<string, Action> providerStates)
        {
            _consumerName = consumerName;
            _providerStates = providerStates;

            return this;
        }

        public IPactProvider ServiceProvider(string providerName, HttpClient httpClient)
        {
            _providerName = providerName;
            _httpClient = httpClient;

            return this;
        }

        public IPactProvider HonoursPactWith(string consumerName)
        {
            _consumerName = consumerName;

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
                var pactFileJson = File.ReadAllText(_pactUri);
                pactFile = JsonConvert.DeserializeObject<PactFile>(pactFileJson, JsonConfig.SerializerSettings);
            }
            catch (IOException)
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

        public void Dispose()
        {
            PersistPactFile();

            if (_mockProviderService != null)
            {
                _mockProviderService.Stop();
                _mockProviderService.Dispose();
            }
        }

        private void PersistPactFile()
        {
            PactFile pactFile;

            try
            {
                var previousPactFileJson = File.ReadAllText(PactFilePath);
                pactFile = JsonConvert.DeserializeObject<PactFile>(previousPactFileJson, JsonConfig.SerializerSettings);
            }
            catch (IOException ex)
            {
                if (ex.GetType() == typeof(DirectoryNotFoundException))
                {
                    Directory.CreateDirectory(PactFileDirectory);
                }

                pactFile = new PactFile
                {
                    Provider = new PactParty { Name = _providerName },
                    Consumer = new PactParty { Name = _consumerName }
                };
            }

            pactFile.Interactions = pactFile.Interactions ?? new List<PactInteraction>();
            pactFile.AddInteraction(_mockProviderService.DescribeInteraction());

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.SerializerSettings);

            File.WriteAllText(PactFilePath, pactFileJson);
        }
    }
}
