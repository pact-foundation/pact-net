using System;
using PactNet;
using PactNet.Mocks.MockHttpService;

namespace Consumer.Tests
{
    public class ConsumerEventApiPact : IDisposable
    {
        public IPactConsumer Pact { get; private set; }
        public IMockProviderService MockProviderService { get; private set; }

        public int MockServerPort { get { return 1234; } }
        public string MockServerBaseUri { get { return String.Format("http://localhost:{0}", MockServerPort); } }

        public ConsumerEventApiPact()
        {
            Pact = new Pact().ServiceConsumer("Consumer")
                .HasPactWith("Event API");

            MockProviderService = Pact.MockService(MockServerPort);
        }

        public void Dispose()
        {
            Pact.Dispose();
        }
    }
}