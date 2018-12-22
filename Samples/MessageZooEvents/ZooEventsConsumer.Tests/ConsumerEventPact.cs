using System;
using System.IO;
using PactNet;
using PactNet.PactMessage;

namespace ZooEventsConsumer.Tests
{
    public class ConsumerEventPact : IDisposable
    {
        public IMessagePactBuilder MessagePactBuilder { get; }
        public IMessagePact MessagePact { get; }

        public ConsumerEventPact()
        {
            MessagePactBuilder = new MessagePactBuilder(new PactConfig
                {
                    SpecificationVersion = "2.0.0",
                    LogDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}",
                    PactDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}"
                })
                .ServiceConsumer("Event Consumer")
                .HasPactWith("Event Producer");

            MessagePact = MessagePactBuilder.InitializePactMessage();
        }

        public void Dispose()
        {
            MessagePactBuilder.Build();
        }
    }
}
