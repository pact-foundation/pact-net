using System;
using System.IO.Abstractions;
using NSubstitute;
using PactNet.Mocks.MockHttpService;

namespace PactNet.Tests.IntegrationTests
{
    public class IntegrationTestsMyApiPact : IDisposable
    {
        public IPactBuilder PactBuilder { get; private set; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort { get { return 4321; } }
        public string MockProviderServiceBaseUri { get { return String.Format("http://localhost:{0}", MockServerPort); } }

        public IntegrationTestsMyApiPact()
        {
            PactBuilder = new PactBuilder((port, enableSsl) => new MockProviderService(port, enableSsl), Substitute.For<IFileSystem>())
                .ServiceConsumer("IntegrationTests")
                .HasPactWith("MyApi");

            MockProviderService = PactBuilder.MockService(MockServerPort);
        }

        public void Dispose()
        {
            PactBuilder.Build();
        }
    }
}