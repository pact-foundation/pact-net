using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using PactNet.Exceptions;
using PactNet.Infrastructure.Outputters;
using PactNet.Native.Verifier;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Native.Tests.Verifier
{
    public class NativePactVerifierPairTests
    {
        private const string ProviderName = "provider";

        private readonly NativePactVerifierPair verifier;

        private readonly Mock<IVerifierProvider> mockProvider;
        private readonly Mock<IOutput> mockOutput;

        private readonly IDictionary<string, string> verifierArgs;

        public NativePactVerifierPairTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();
            this.mockProvider.Setup(p => p.Verify(It.IsAny<string>())).Returns(PactVerifierResult.Success);
            this.mockProvider.Setup(p => p.VerifierLogs(It.IsAny<string>())).Returns(string.Empty);

            this.mockOutput = new Mock<IOutput>();

            this.verifierArgs = new Dictionary<string, string>
            {
                ["--provider-name"] = ProviderName
            };

            var config = new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output),
                    this.mockOutput.Object
                }
            };

            this.verifier = new NativePactVerifierPair(this.verifierArgs, this.mockProvider.Object, config);
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

        [Fact]
        public void Verify_Success_DoesNotThrow()
        {
            this.mockProvider.Setup(p => p.Verify(It.IsAny<string>())).Returns(PactVerifierResult.Success);

            Action action = () => this.verifier.Verify();

            action.Should().NotThrow("because the verification succeeded");
        }

        [Theory]
        [InlineData(PactVerifierResult.InvalidArguments)]
        [InlineData(PactVerifierResult.Failure)]
        [InlineData(PactVerifierResult.NullPointer)]
        [InlineData(PactVerifierResult.Panic)]
        [InlineData(PactVerifierResult.UnknownError)]
        internal void Verify_Error_ThrowsPactFailureException(PactVerifierResult result)
        {
            this.mockProvider.Setup(p => p.Verify(It.IsAny<string>())).Returns(result);

            Action action = () => this.verifier.Verify();

            action.Should().Throw<PactFailureException>();
        }

        [Fact]
        public void Verify_UnexpectedResult_ThrowsArgumentOutOfRangeException()
        {
            this.mockProvider.Setup(p => p.Verify(It.IsAny<string>())).Returns((PactVerifierResult)12345);

            Action action = () => this.verifier.Verify();

            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Verify_WhenCalled_AddsLogsToOutput()
        {
            const string expected = "logs";
            this.mockProvider.Setup(p => p.VerifierLogs(ProviderName)).Returns(expected);

            this.verifier.Verify();

            this.mockOutput.Verify(o => o.WriteLine(expected));
        }

        private void CheckArgs(params string[] args)
        {
            args = new[] { "--provider-name", ProviderName }.Concat(args).ToArray();

            this.verifier.Verify();

            string formatted = string.Join(Environment.NewLine, args);

            this.mockProvider.Verify(v => v.Verify(formatted));
        }
    }
}
