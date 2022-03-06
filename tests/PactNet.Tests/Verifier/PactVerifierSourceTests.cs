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
        public void WithFilter_WhenCalled_SetsFilterInfo()
        {
            this.verifier.WithFilter("description", "provider state");

            this.mockProvider.Verify(p => p.SetFilterInfo("description", "provider state", null));
        }

        [Fact]
        public void WithRequestTimeout_WhenCalled_SetsRequestTimeout()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(1);

            this.verifier.WithRequestTimeout(timeout);
            this.verifier.Verify();

            this.mockProvider.Verify(p => p.SetVerificationOptions(It.IsAny<bool>(), timeout));
        }

        [Fact]
        public void WithSslVerificationDisabled_WhenCalled_DisablesSslVerification()
        {
            this.verifier.WithSslVerificationDisabled();
            this.verifier.Verify();

            this.mockProvider.Verify(p => p.SetVerificationOptions(true, It.IsAny<TimeSpan>()));
        }

        [Fact]
        public void WithCustomHeader_WhenCalled_AddsCustomHeader()
        {
            this.verifier.WithCustomHeader("Authorization", "Bearer abcdef0123456789");
            this.verifier.Verify();

            this.mockProvider.Verify(p => p.AddCustomHeader("Authorization", "Bearer abcdef0123456789"));
        }

        [Fact]
        public void Verify_WithoutRequestTimeout_UsesDefaultTimeout()
        {
            this.verifier.Verify();

            this.mockProvider.Verify(p => p.SetVerificationOptions(It.IsAny<bool>(), TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public void Verify_WithoutDisablingSslVerification_EnablesSslVerification()
        {
            this.verifier.Verify();

            this.mockProvider.Verify(p => p.SetVerificationOptions(false, It.IsAny<TimeSpan>()));
        }

        [Fact]
        public void Verify_WhenCalled_Verifies()
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
