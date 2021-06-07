using System;
using System.Collections.Generic;
using System.IO;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Native;
using Provider.Api.Web.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class EventApiTests : IClassFixture<EventApiFixture>
    {
        private readonly EventApiFixture fixture;
        private readonly ITestOutputHelper output;

        public EventApiTests(EventApiFixture fixture, ITestOutputHelper output)
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

            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "Consumer.Tests",
                                           "pacts",
                                           "Event API Consumer-Event API.json");

            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ServiceProvider("Event API", this.fixture.ServerUri)
                .HonoursPactWith("Event API Consumer")
                .FromPactFile(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(this.fixture.ServerUri, "/provider-states"))
                .Verify();
        }
    }
}
