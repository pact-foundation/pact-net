using System;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace Consumer.Tests
{
    public class ConsumerEventApiPact : IDisposable
    {
        public IPactBuilder PactBuilder { get; private set; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort { get { return 12345; } }
        public string MockProviderServiceBaseUri { get { return String.Format("http://localhost:{0}", MockServerPort); } }

        public ConsumerEventApiPact()
        {
            PactBuilder = new PactBuilder()
                .ServiceConsumer("Consumer")
                .HasPactWith("Event API");

            MockProviderService = PactBuilder.MockService(MockServerPort);
        }

        public void Dispose()
        {
            PactBuilder.Build();
        }
    }
}