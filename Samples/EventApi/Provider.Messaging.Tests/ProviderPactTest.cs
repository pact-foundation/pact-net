using PactNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.Messaging.Tests
{
    public class ProviderPactTest
    {
        private class PactConnectionInfo
        {
            public string Uri { get; set; }
            public PactUriOptions Options { get; set; }
        }

        public void VerifyConsumerPact()
        {
            var config = new PactVerifierConfig();

            Provider.Messaging.Models.Event party = new Models.Event();

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
                Uri = string.Empty,
                Options = new PactUriOptions(string.Empty, string.Empty)
            };

            return info;
        }
    }
}
