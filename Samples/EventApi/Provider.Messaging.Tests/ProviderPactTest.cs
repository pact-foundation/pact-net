using PactNet;
using Provider.Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Provider.Messaging.Tests
{
    public class ProviderPactTest
    {
        private class PactConnectionInfo
        {
            public string Uri { get; set; }
            public PactUriOptions Options { get; set; }
        }

        [Fact]
        public void VerifyConsumerPact()
        {
            Event party = new Event()
            {
                EventId = Guid.NewGuid(),
                EventType = "Party",
                Timestamp = DateTime.UtcNow,
                Location = new Location()
                {
                    Latitude = new Coordinate() { Degrees = 278, Minutes = 10, Seconds = 7.8 },
                    Longitude = new Coordinate() { Degrees = 8, Minutes = 64, Seconds = 12.0 }
                }
            };

            PactMessagingVerifier verifier = new PactMessagingVerifier();

            verifier.IAmProvider("Provider.Messaging-dotNet")
                .BroadCast("event.party", string.Empty, party)
                .HonoursPactWith("Consumer-dotNet")
                .PactUri("../../../Consumer.Tests/pacts/consumer-dotnet-provider.messaging-dotnet.json")
                .Verify();
        }

        private PactConnectionInfo GetPactConnectionInfo()
        {
            //You should read this from a config or environment var.
            PactConnectionInfo info = new PactConnectionInfo()
            {
                Uri = "https://yourpackbroker.com/",
            Options = new PactUriOptions("username", "password")
            };

            return info;
        }
    }
}
