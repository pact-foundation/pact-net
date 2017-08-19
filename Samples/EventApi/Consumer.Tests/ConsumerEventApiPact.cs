using System;
using System.IO;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace Consumer.Tests
{
    public class ConsumerEventApiPact : IDisposable
    {
        public IPactBuilder PactBuilder { get; }
        public IMockProviderService MockProviderService { get; }

        public int MockServerPort => 9222;
        public string MockProviderServiceBaseUri => $"http://localhost:{MockServerPort}";

        public ConsumerEventApiPact()
        {
            PactBuilder = new PactBuilder(new PactConfig
                {
                    SpecificationVersion = "2.0.0",
                    LogDir = Path.GetFullPath(@"..\..\..\logs\"),
                    PactDir = Path.GetFullPath(@"..\..\..\pacts\")
                })
                .ServiceConsumer("Event API Consumer")
                .HasPactWith("Event API");

            MockProviderService = PactBuilder.MockService(MockServerPort);
        }

        public void Dispose()
        {
            PactBuilder.Build();
        }
    }
}