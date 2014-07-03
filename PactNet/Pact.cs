using System;
using System.Collections.Generic;
using System.IO;
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
        private IMockProviderService _mockProviderService;
        private const string PactFileDirectory = "../../pacts/";
        private HttpClient _client;

        private string PactFilePath
        {
            get { return Path.Combine(PactFileDirectory, PactFileName); }
        }

        private string PactFileName
        {
            get { return String.Format("{0}-{1}.json", _consumerName, _providerName).Replace(' ', '_').ToLower(); }
        }

        public Pact()
        {
        }

        public Pact(HttpClient client)
        {
            _client = client;
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

        public IPactProvider ServiceProvider(string providerName)
        {
            _providerName = providerName;

            return this;
        }

        public IPactProvider HonoursPactWith(string consumerName)
        {
            _consumerName = consumerName;

            return this;
        }

        public IPactProvider PactUri(string uri)
        {
            try
            {
                var pactFileJson = File.ReadAllText(uri);
                var pactFile = JsonConvert.DeserializeObject<PactFile>(pactFileJson, JsonConfig.SerializerSettings);
                pactFile.VerifyProvider(_client);
            }
            catch (IOException)
            {
                throw new PactAssertException(String.Format("Json Pact file could not be retrieved using uri \'{0}\'", uri));
            }

            return this;
        }

        public void Dispose()
        {
            PersistPactFile();

            _mockProviderService.Stop();
            _mockProviderService.Dispose();
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
