using System;
using System.Collections.Generic;
using System.IO;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
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
                LogLevel = PactLogLevel.Information,
                Outputters = new List<IOutput>
                {
                    new XunitOutput(this.output)
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
            IPactVerifier verifier = new PactVerifier(config);
            verifier
                .ServiceProvider("Event API", this.fixture.ServerUri)
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(this.fixture.ServerUri, "/provider-states"))
                .WithRequestTimeout(TimeSpan.FromSeconds(2))
                .WithSslVerificationDisabled()
                .Verify();
        }
    }
}
