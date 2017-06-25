using System;
using System.Configuration;
using System.IO;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace Consumer.Tests
{
    public class ConsumerEventApiPact : IDisposable
    {
        public IPactBuilder PactBuilder { get; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort => 9222;
        public string MockProviderServiceBaseUri => $"http://localhost:{MockServerPort}";

        public ConsumerEventApiPact()
        {
            if (!File.Exists(".\\pact\\bin\\pact-mock-service.bat"))
            {
                throw new Exception("Please run '.\\Build\\Download-Standalone-Core.ps1' from the project root to download the standalone mock provider service.");
            }

            PactBuilder = new PactBuilder(new PactConfig { SpecificationVersion = "2.0.0" })
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