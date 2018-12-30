using System;
using System.Collections.Generic;
using System.IO;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.PactMessage;
using Xunit.Abstractions;

namespace ZooEventsConsumer.Tests
{
    public class ConsumerEventPact : IDisposable
    {
        private IMessagePactBuilder _messagePactBuilder;

        public IMessagePact Initialise(ITestOutputHelper output)
        {
            _messagePactBuilder = new MessagePactBuilder(new PactConfig
                {
                    SpecificationVersion = "2.0.0",
                    LogDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}logs{Path.DirectorySeparatorChar}",
                    PactDir = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}",
                    Outputters = new List<IOutput>
                    {
                        new XUnitOutput(output)
                    }
                })
                .ServiceConsumer("Zoo Event Consumer")
                .HasPactWith("Zoo Event Producer");

            return _messagePactBuilder.InitializePactMessage();
        }

        public void Dispose()
        {
            _messagePactBuilder.Build();
        }
    }
}
