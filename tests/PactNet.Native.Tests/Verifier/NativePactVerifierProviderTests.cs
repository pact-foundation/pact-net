using System.Collections.Generic;
using FluentAssertions;
using PactNet.Native.Verifier;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Native.Tests.Verifier
{
    public class NativePactVerifierProviderTests
    {
        private readonly NativePactVerifierProvider verifier;

        private readonly IDictionary<string, string> verifierArgs;

        public NativePactVerifierProviderTests(ITestOutputHelper output)
        {
            this.verifierArgs = new Dictionary<string, string>();

            this.verifier = new NativePactVerifierProvider(this.verifierArgs, new PactVerifierConfig
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
