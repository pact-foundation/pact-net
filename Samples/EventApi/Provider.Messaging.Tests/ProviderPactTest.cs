using PactNet;
using Provider.Messaging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var config = new PactVerifierConfig();

            Provider.Messaging.Models.Event party = new Models.Event()
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

            PactMessagingVerifier verifier = new PactMessagingVerifier(config);


            PactConnectionInfo connectionInfo = GetPactConnectionInfo();

            verifier.IAmProvider("Provider")
                .BroadCast("my.random.topic", string.Empty, party)
                .HonoursPactWith("Consumer")
                .PactUri(connectionInfo.Uri, connectionInfo.Options)
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
