using System;
using System.Collections.Generic;
using System.IO;
using PactNet;
using PactNet.Infrastructure.Outputters;
using Xunit;
using Xunit.Abstractions;

namespace ZooEventsProducer.Tests
{
    public class ZooProducerTests
    {
        private readonly ITestOutputHelper _output;

        public ZooProducerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EnsureEventProducerHonoursPactWithEventConsumer()
        {
            //Arrange
            const string serviceUri = "http://localhost:9222";
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_output)
                }
            };

            //using (WebApp.Start<TestStartup>(serviceUri))
            //{
                //Act / Assert
                IPactVerifier pactVerifier = new PactVerifier(config);
                pactVerifier
                    .ServiceProvider("Event API", serviceUri)
                    .HonoursPactWith("Event API Message Consumer")
                    .PactUri($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Consumer.Tests{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}event_api_message_consumer-event_api.json")
                    .Verify();
            //}
        }
    }
}
