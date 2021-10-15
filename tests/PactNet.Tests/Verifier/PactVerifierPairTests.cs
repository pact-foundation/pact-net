using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierPairTests
    {
        private readonly PactVerifierPair verifier;

        private readonly Mock<IVerifierProvider> mockProvider;

        private readonly IDictionary<string, string> verifierArgs;

        public PactVerifierPairTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            this.verifierArgs = new Dictionary<string, string>();

            var config = new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            };

            this.verifier = new PactVerifierPair(this.verifierArgs, this.mockProvider.Object, config);
        }

        [Fact]
        public void WithProviderStateUrl_WhenCalled_SetsProviderStatePath()
        {
            this.verifier.WithProviderStateUrl(new Uri("http://example.org/provider/state/path/"));

            this.CheckArgs("--state-change-url", "http://example.org/provider/state/path/");
        }

        [Fact]
        public void WithFilter_WhenCalled_AddsFilterArgs()
        {
            this.verifier.WithFilter("description", "provider state");

            this.CheckArgs("--filter-description", "description",
                           "--filter-state", "provider state");
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

            this.CheckArgs("--loglevel", expected);
        }

        [Fact]
        public void WithLogLevel_InvalidLevel_ThrowsArgumentOutOfRangeException()
        {
            Action action = () => this.verifier.WithLogLevel((PactLogLevel)12345);

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Verify_FlagWithoutArgument_FormatsArgumentsProperly()
        {
            this.verifierArgs.Add("--foo", string.Empty);
            this.verifierArgs.Add("--bar", string.Empty);
            this.verifierArgs.Add("--baz", string.Empty);

            this.CheckArgs("--foo", "--bar", "--baz");
        }

        private void CheckArgs(params string[] args)
        {
            this.verifier.Verify();

            string formatted = string.Join(Environment.NewLine, args);

            this.mockProvider.Verify(v => v.Verify(formatted));
        }
    }
}
