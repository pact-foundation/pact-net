/*using System;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Host;

namespace PactNet.Tests.IntegrationTests
{
    public class IntegrationTestsMyApiPact : IDisposable
    {
        public IPactBuilder PactBuilder { get; }
        public IMockProviderService MockProviderService { get; }

        public int MockServerPort => 4322;
        public Uri MockProviderServiceBaseUri => new Uri($"http://localhost:{MockServerPort}");

        public IntegrationTestsMyApiPact()
        {
            var pactConfig = new PactConfig();

            PactBuilder = new PactBuilder(
                    (port, enableSsl, consumerName, providerName, host, JsonSerializerOptions, sslCert, sslKey) =>
                        new MockProviderService(
                            baseUri => new RubyHttpHost(baseUri, "MyConsumer", "MyApi", pactConfig, host),
                            port, enableSsl,
                            baseUri => new AdminHttpClient(baseUri, JsonSerializerOptions)))
                .ServiceConsumer("IntegrationTests")
                .HasPactWith("MyApi");

            MockProviderService = PactBuilder.MockService(MockServerPort);
        }

        public void Dispose()
        {
            PactBuilder.Build();
        }
    }
}*/
