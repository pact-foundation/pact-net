using System;
using System.IO.Abstractions;
using System.Net.Http;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Validators;

namespace PactNet
{
    public partial class Pact
    {
        private readonly IFileSystem _fileSystem;

        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }

        [Obsolete("For PactProvider testing only.")]
        public Pact(Func<int, IMockProviderService> mockProviderServiceFactory, IFileSystem fileSystem, Func<HttpClient, IProviderServiceValidator> providerServiceValidatorFactory)
        {
            _mockProviderServiceFactory = mockProviderServiceFactory;
            _fileSystem = fileSystem;
            _providerServiceValidatorFactory = providerServiceValidatorFactory;
        }

        [Obsolete("For PactConsumer testing only.")]
        public Pact(Func<int, IMockProviderService> mockProviderServiceFactory, IFileSystem fileSystem)
            : this(
                mockProviderServiceFactory,
                fileSystem,
                httpClient => new ProviderServiceValidator(httpClient)
            )
        {
        }

        public Pact()
            : this(
                port => new MockProviderService(port),
                new FileSystem())
        {
        }
    }
}