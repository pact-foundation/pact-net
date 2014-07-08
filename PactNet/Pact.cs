using System;
using System.IO.Abstractions;
using PactNet.Consumer.Mocks.MockService;

namespace PactNet
{
    public partial class Pact
    {
        private readonly IFileSystem _fileSystem;

        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }

        [Obsolete("For testing purposes only")]
        public Pact(Func<int, IMockProviderService> mockProviderServiceFactory, IFileSystem fileSystem)
        {
            _mockProviderServiceFactory = mockProviderServiceFactory;
            _fileSystem = fileSystem;
        }

        public Pact()
            : this(
                port => new MockProviderService(port),
                new FileSystem())
        {
        }
    }
}