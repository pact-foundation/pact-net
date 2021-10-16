using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierConsumerTests
    {
        private readonly PactVerifierConsumer verifier;

        private readonly IDictionary<string, string> verifierArgs;

        public PactVerifierConsumerTests(ITestOutputHelper output)
        {
            this.verifierArgs = new Dictionary<string, string>();

            this.verifier = new PactVerifierConsumer(this.verifierArgs, new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            });
        }

        [Fact]
        public void FromPactFile_WhenCalled_AddsFileArg()
        {
            var file = new FileInfo("/home/user/pacts/file.json");
            this.verifier.FromPactFile(file);

            this.verifierArgs.Should().Contain("--file", file.FullName);
        }

        [Fact]
        public void FromPactUri_WhenCalled_AddsUrlArg()
        {
            this.verifier.FromPactUri(new Uri("http://example.org/pact/file.json"));

            this.verifierArgs.Should().Contain("--url", "http://example.org/pact/file.json");
        }

        [Fact]
        public void FromPactBroker_WhenCalled_AddsPactBrokerArg()
        {
            this.verifier.FromPactBroker(new Uri("http://broker.example.org/"));

            this.verifierArgs.Should().Contain("--broker-url", "http://broker.example.org/");
        }
    }
}
