using System;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Consumer;
using PactNet.Consumer.Mocks.MockService;

namespace PactNet
{
    //TODO: Implement a Pact file broker
    public partial class Pact : IPactConsumer
    {
        private readonly Func<int, IMockProviderService> _mockProviderServiceFactory;
        private IMockProviderService _mockProviderService;
        private const string PactFileDirectory = "../../pacts/";

        public string PactFilePath
        {
            get { return _fileSystem.Path.Combine(PactFileDirectory, PactFileName); }
        }

        private string PactFileName
        {
            get { return String.Format("{0}-{1}.json", ConsumerName, ProviderName).Replace(' ', '_').ToLower(); }
        }

        public IPactConsumer ServiceConsumer(string consumerName)
        {
            if (String.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            ConsumerName = consumerName;

            return this;
        }

        public IPactConsumer HasPactWith(string providerName)
        {
            if (String.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            ProviderName = providerName;

            return this;
        }

        public IMockProviderService MockService(int port)
        {
            if (_mockProviderService != null)
            {
                _mockProviderService.Stop();
            }

            _mockProviderService = _mockProviderServiceFactory(port);

            _mockProviderService.Start();

            return _mockProviderService;
        }

        public void Dispose()
        {
            PersistPactFile();

            if (_mockProviderService != null)
            {
                _mockProviderService.Stop();
            }
        }

        private void PersistPactFile()
        {
            var pactFile = new PactFile
            {
                Provider = new PactParty { Name = ProviderName },
                Consumer = new PactParty { Name = ConsumerName }
            };

            if (_mockProviderService != null)
            {
                pactFile.AddInteractions(_mockProviderService.Interactions);
            }

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.SerializerSettings);

            try
            {
                _fileSystem.File.WriteAllText(PactFilePath, pactFileJson);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                _fileSystem.Directory.CreateDirectory(PactFileDirectory);
                _fileSystem.File.WriteAllText(PactFilePath, pactFileJson);
            }
        }
    }
}
