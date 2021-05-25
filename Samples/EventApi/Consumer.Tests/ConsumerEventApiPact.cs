using System;
using PactNet;

namespace Consumer.Tests
{
    public class ConsumerEventApiPact : IDisposable
    {
        public PactConfig Config { get; }
        public IPactBuilder PactBuilder { get; }
        public IInteractionBuilder Interactions { get; }

        public ConsumerEventApiPact()
        {
            this.Config = new PactConfig
            {
                SpecificationVersion = "2.0.0",
                LogDir = "../../../logs/",
                PactDir = "../../../pacts/"
            };

            this.PactBuilder = new PactBuilder(this.Config)
                               .ServiceConsumer("Event API Consumer")
                               .HasPactWith("Event API");

            this.Interactions = this.PactBuilder.UsingNativeBackend(port: 6868);
        }

        public void Dispose()
        {
            this.PactBuilder.Build();
        }
    }
}