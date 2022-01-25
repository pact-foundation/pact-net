using System;
using FluentAssertions;
using Moq;
using PactNet.Exceptions;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierSourceTests
    {
        private readonly PactVerifierSource verifier;

        private readonly Mock<IVerifierProvider> mockProvider;

        public PactVerifierSourceTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            var config = new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            };

            this.verifier = new PactVerifierSource(this.mockProvider.Object, config);
        }

        [Fact]
        public void WithProviderStateUrl_WhenCalled_SetsProviderStatePath()
        {
            var uri = new Uri("http://example.org/provider/state/path/");

            this.verifier.WithProviderStateUrl(uri);

            this.mockProvider.Verify(p => p.SetProviderState(uri, false, true));
        }

        [Fact]
        public void WithFilter_WhenCalled_AddsFilterArgs()
        {
            this.verifier.WithFilter("description", "provider state");

            this.mockProvider.Verify(p => p.SetFilterInfo("description", "provider state", null));
        }

        [Fact]
        public void Verify_WhenCalled_VerifiesWithFormattedArguments()
        {
            this.verifier.Verify();

            this.mockProvider.Verify(p => p.Execute());
        }

        [Fact]
        public void Verify_VerificationError_ThrowsPactFailureException()
        {
            this.mockProvider.Setup(p => p.Execute()).Throws(new PactFailureException("Uh oh"));

            Action action = () => this.verifier.Verify();

            action.Should().Throw<PactFailureException>();
        }
    }
}
