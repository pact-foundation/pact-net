using System.Collections.Generic;
using FluentAssertions;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierProviderTests
    {
        private readonly PactVerifierProvider verifier;

        private readonly IDictionary<string, string> verifierArgs;

        public PactVerifierProviderTests(ITestOutputHelper output)
        {
            this.verifierArgs = new Dictionary<string, string>();

            this.verifier = new PactVerifierProvider(this.verifierArgs, new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            });
        }

        [Fact]
        public void HonoursPactWith_WhenCalled_AddsConsumerArgs()
        {
            this.verifier.HonoursPactWith("consumer name");

            this.verifierArgs.Should().Contain("--filter-consumer", "consumer name");
        }
    }
}
