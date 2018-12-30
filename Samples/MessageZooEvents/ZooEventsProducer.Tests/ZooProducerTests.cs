using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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

            var builder = WebHost.CreateDefaultBuilder()
                .UseUrls(serviceUri)
                .PreferHostingUrls(true)
                .UseStartup<TestStartup>();

            using (var host = builder.Build())
            {
                host.Start();
            
                //Act / Assert
                IPactVerifier pactVerifier = new PactVerifier(config);
                    pactVerifier
                        .ServiceProvider("Zoo Event Producer", serviceUri)
                        .HonoursPactWith("Zoo Event Consumer")
                        .PactUri($"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}ZooEventsConsumer.Tests{Path.DirectorySeparatorChar}pacts{Path.DirectorySeparatorChar}zoo_event_consumer-zoo_event_producer.json")
                        .Verify();
            }
        }
    }
}
