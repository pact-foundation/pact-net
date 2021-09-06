using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Native;
using Xunit;
using Xunit.Abstractions;

namespace ReadMe.Provider.Tests
{
    public class SomethingApiTests : IClassFixture<SomethingApiFixture>
    {
        private readonly SomethingApiFixture fixture;
        private readonly ITestOutputHelper output;

        public SomethingApiTests(SomethingApiFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
        }

        [Fact]
        public void EnsureSomethingApiHonoursPactWithConsumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    // NOTE: PactNet defaults to a ConsoleOutput, however
                    // xUnit 2 does not capture the console output, so this
                    // sample creates a custom xUnit outputter. You will
                    // have to do the same in xUnit projects.
                    new XUnitOutput(output),
                },
            };

            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "pacts",
                                           "Something API Consumer-Something API.json");

            // Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ServiceProvider("Something API", fixture.ServerUri)
                .HonoursPactWith("Something API Consumer")
                .FromPactFile(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(fixture.ServerUri, "/provider-states"))
                .Verify();
        }
    }
}
