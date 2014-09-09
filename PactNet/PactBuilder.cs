using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Newtonsoft.Json;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;

namespace PactNet
{
    public class PactBuilder : IPactBuilder
    {
        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }
        private readonly IFileSystem _fileSystem;
        private readonly Func<int, bool, IMockProviderService> _mockProviderServiceFactory;
        private IMockProviderService _mockProviderService;
        private const string PactFileDirectory = "../../pacts/";

        private string PactFileName
        {
            get { return String.Format("{0}-{1}.json", ConsumerName, ProviderName).Replace(' ', '_').ToLower(); }
        }

        public string PactFileUri
        {
            get
            {
                return _fileSystem.Path.Combine(PactFileDirectory, PactFileName);
            }
        }

        internal PactBuilder(
            Func<int, bool, IMockProviderService> mockProviderServiceFactory, 
            IFileSystem fileSystem)
        {
            _mockProviderServiceFactory = mockProviderServiceFactory;
            _fileSystem = fileSystem;
        }

        public PactBuilder()
            : this(
                (port, enableSsl) => new MockProviderService(port, enableSsl),
                new FileSystem())
        {
        }

        public IPactBuilder ServiceConsumer(string consumerName)
        {
            if (String.IsNullOrEmpty(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            ConsumerName = consumerName;

            return this;
        }

        public IPactBuilder HasPactWith(string providerName)
        {
            if (String.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            ProviderName = providerName;

            return this;
        }

        public IMockProviderService MockService(int port, bool enableSsl = false)
        {
            if (_mockProviderService != null)
            {
                _mockProviderService.Stop();
            }

            _mockProviderService = _mockProviderServiceFactory(port, enableSsl);

            _mockProviderService.Start();

            return _mockProviderService;
        }

        public void Build()
        {
            PersistPactFile();

            if (_mockProviderService != null)
            {
                _mockProviderService.Stop();
            }
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

            var pactFile = new ProviderServicePactFile
            {
                Provider = new Party { Name = ProviderName },
                Consumer = new Party { Name = ConsumerName }
            };

            if (_mockProviderService != null)
            {
                var interactions = _mockProviderService.Interactions;
                if (interactions != null)
                {
                    pactFile.Interactions = interactions as IEnumerable<ProviderServiceInteraction>;
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
