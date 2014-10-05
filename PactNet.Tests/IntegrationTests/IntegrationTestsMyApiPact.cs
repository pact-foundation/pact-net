using System;
using System.Net.Http;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Nancy;

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
            PactBuilder = new PactBuilder((port, enableSsl) =>
                    new MockProviderService(
                        baseUri => new NancyHttpHost(baseUri, new IntegrationTestingMockProviderNancyBootstrapper()),
                        port, enableSsl,
                        baseUri => new HttpClient { BaseAddress = new Uri(baseUri) }))
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