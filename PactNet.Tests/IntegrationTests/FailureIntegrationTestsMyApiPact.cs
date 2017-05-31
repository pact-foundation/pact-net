using System;
using System.Net.Http;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Mappers;
using PactNet.Mocks.MockHttpService.Ruby;

namespace PactNet.Tests.IntegrationTests
{
    public class FailureIntegrationTestsMyApiPact : IDisposable
    {
        public IPactBuilder PactBuilder { get; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort => 4321;
        public Uri MockProviderServiceBaseUri => new Uri($"http://localhost:{MockServerPort}");

        public FailureIntegrationTestsMyApiPact()
        {
            var pactConfig = new PactConfig();

            PactBuilder = new PactBuilder((port, enableSsl, providerName) => 
                    new MockProviderService(
                        baseUri => new RubyHttpHost(baseUri, "MyApi",  pactConfig), 
                        port, enableSsl, 
                        baseUri => new HttpClient { BaseAddress = baseUri },
                        new HttpMethodMapper()))
                .ServiceConsumer("FailureIntegrationTests")
                .HasPactWith("MyApi");

            MockProviderService = PactBuilder.MockService(MockServerPort);
        }

        public void Dispose()
        {
            PactBuilder.Build();
        }
    }
}