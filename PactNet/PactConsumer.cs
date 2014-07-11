using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;

namespace PactNet
{
    //TODO: Implement a Pact file broker
    public partial class Pact : IPactConsumer
    {
        private readonly Func<int, IMockProviderService> _mockProviderServiceFactory;
        private IMockProviderService _mockProviderService;
        private const string PactFileDirectory = "../../pacts/";

        private string PactFileName
        {
            get { return String.Format("{0}-{1}.json", ConsumerName, ProviderName).Replace(' ', '_').ToLower(); }
        }

        private string _pactFileUri;
        public string PactFileUri
        {
            private set { _pactFileUri = value; }
            get
            {
                if (String.IsNullOrEmpty(_pactFileUri))
                {
                    return _fileSystem.Path.Combine(PactFileDirectory, PactFileName);
                }

                return _pactFileUri;
            }
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
            if (_mockProviderService != null)
            {
                _mockProviderService.Stop();
            }

            PersistPactFile();
        }

        private void PersistPactFile()
        {
            if (String.IsNullOrEmpty(ConsumerName))
            {
                throw new InvalidOperationException("ConsumerName has not been set, please supply a consumer name using the ServiceConsumer method.");
            }

            if (String.IsNullOrEmpty(ProviderName))
            {
                throw new InvalidOperationException("ProviderName has not been set, please supply a provider name using the HasPactWith method.");
            }

            var pactFile = new ServicePactFile
            {
                Provider = new PactParty { Name = ProviderName },
                Consumer = new PactParty { Name = ConsumerName }
            };

            if (_mockProviderService != null)
            {
                var interactions = _mockProviderService.Interactions;
                if (interactions != null)
                {
                    pactFile.Interactions = interactions as IEnumerable<PactServiceInteraction>;
                }
            }

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonConfig.PactFileSerializerSettings);

            try
            {
                _fileSystem.File.WriteAllText(PactFileUri, pactFileJson);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                _fileSystem.Directory.CreateDirectory(PactFileDirectory);
                _fileSystem.File.WriteAllText(PactFileUri, pactFileJson);
            }
        }
    }
}
