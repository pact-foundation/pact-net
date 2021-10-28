using System;
using FluentAssertions;
using Moq;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierSourceTests
    {
        private readonly PactVerifierSource verifier;

        private readonly Mock<IVerifierProvider> mockProvider;
        private readonly Mock<IVerifierArguments> verifierArgs;

        public PactVerifierSourceTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();
            this.verifierArgs = new Mock<IVerifierArguments>();

            var config = new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            };

            this.verifier = new PactVerifierSource(this.verifierArgs.Object, this.mockProvider.Object, config);
        }

        [Fact]
        public void WithProviderStateUrl_WhenCalled_SetsProviderStatePath()
        {
            this.verifier.WithProviderStateUrl(new Uri("http://example.org/provider/state/path/"));

            this.verifierArgs.Verify(a => a.AddOption("--state-change-url", "http://example.org/provider/state/path/", "providerStateUri"));
        }

        [Fact]
        public void WithFilter_WhenCalled_AddsFilterArgs()
        {
            this.verifier.WithFilter("description", "provider state");

            this.verifierArgs.Verify(a => a.AddOption("--filter-description", "description", "description"));
            this.verifierArgs.Verify(a => a.AddOption("--filter-state", "provider state", "providerState"));
        }

        [Theory]
        [InlineData(PactLogLevel.Trace,       "trace")]
        [InlineData(PactLogLevel.Debug,       "debug")]
        [InlineData(PactLogLevel.Information, "info")]
        [InlineData(PactLogLevel.Warn,        "warn")]
        [InlineData(PactLogLevel.Error,       "error")]
        [InlineData(PactLogLevel.None,        "none")]
        public void WithLogLevel_WhenCalled_AddsLogLevelArg(PactLogLevel level, string expected)
        {
            this.verifier.WithLogLevel(level);

            this.verifierArgs.Verify(a => a.AddOption("--loglevel", expected, null));
        }

        [Fact]
        public void WithLogLevel_InvalidLevel_ThrowsArgumentOutOfRangeException()
        {
            Action action = () => this.verifier.WithLogLevel((PactLogLevel)12345);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Verify_WhenCalled_VerifiesWithFormattedArguments()
        {
            const string expected = "arguments";
            this.verifierArgs.Setup(a => a.ToString()).Returns(expected);

            this.verifier.Verify();

            this.mockProvider.Verify(p => p.Verify(expected));
        }
    }
}
