using System;
using System.Collections.Generic;
using System.IO;

using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Native;

using Xunit;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class EventImporterProviderTests : IClassFixture<EventApiFixture>
    {
        private readonly EventApiFixture fixture;
        private readonly ITestOutputHelper output;

        public EventImporterProviderTests(EventApiFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
        }

        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(this.output)
                }
            };

            //C
            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "Consumer.Tests",
                                           "pacts",
                                           "Event API Consumer V3 Message-Event API V3 Message.json");

            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ServiceProvider("Event API V3 Message", this.fixture.ServerUri)
                .HonoursPactWith("Event API Consumer V3 Message")
                .FromPactFile(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(this.fixture.ServerUri, "/provider-states"))
                .Verify();
        }
    }
}
