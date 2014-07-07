using System;
using PactNet;
using PactNet.Consumer;
using PactNet.Consumer.Mocks;

namespace Consumer.Tests
{
    public class ConsumerEventApiPact : IDisposable
    {
        public IPactConsumer Pact { get; private set; }
        public IMockProvider MockProvider { get; private set; }

        public int MockServerPort { get { return 1234; } }
        public string MockServerBaseUri { get { return String.Format("http://localhost:{0}", MockServerPort); } }

        public ConsumerEventApiPact()
        {
            Pact = new Pact().ServiceConsumer("Consumer")
                .HasPactWith("Event API");

            MockProvider = Pact.MockService(MockServerPort);
        }

        public void Dispose()
        {
            Pact.Dispose();
        }
    }
}