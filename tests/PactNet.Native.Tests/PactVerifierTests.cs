using System;
using System.IO;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Native.Tests
{
    public class PactVerifierTests
    {
        private readonly PactVerifier verifier;

        private readonly Mock<IVerifierProvider> mockProvider;
        private readonly PactVerifierConfig config;

        public PactVerifierTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();
            this.config = new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            };

            this.verifier = new PactVerifier(this.mockProvider.Object, this.config);
        }

        [Fact]
        public void ServiceProvider_NoBasePath_SetsProviderArgs()
        {
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/"));

            this.CheckArgs("--provider-name", "provider name",
                           "--hostname", "example.org",
                           "--port", "8080");
        }

        [Fact]
        public void ServiceProvider_BasePath_SetsProviderArgs()
        {
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/base/path"));

            this.CheckArgs("--provider-name", "provider name",
                           "--hostname", "example.org",
                           "--port", "8080",
                           "--base-path", "/base/path");
        }

        [Fact]
        public void HonoursPactWith_WhenCalled_AddsConsumerArgs()
        {
            this.verifier.HonoursPactWith("consumer name");

            this.CheckArgs("--filter-consumer", "consumer name");
        }

        [Fact]
        public void FromPactFile_WhenCalled_AddsFileArg()
        {
            var file = new FileInfo("/home/user/pacts/file.json");
            this.verifier.FromPactFile(file);

            this.CheckArgs("--file", file.FullName);
        }

        [Fact]
        public void FromPactUri_WhenCalled_AddsUrlArg()
        {
            this.verifier.FromPactUri(new Uri("http://example.org/pact/file.json"));

            this.CheckArgs("--url", "http://example.org/pact/file.json");
        }

        [Fact]
        public void FromPactBroker_WhenCalled_AddsPactBrokerArg()
        {
            this.verifier.FromPactBroker(new Uri("http://broker.example.org/"));

            this.CheckArgs("--broker-url", "http://broker.example.org/");
        }

        [Fact]
        public void FromPactBroker_BasicAuth_AddsPactBrokerAuthArgs()
        {
            this.verifier.FromPactBroker(new Uri("http://broker.example.org/"), new PactUriOptions("username", "password"));

            this.CheckArgs("--broker-url", "http://broker.example.org/",
                           "--user", "username",
                           "--password", "password");
        }

        [Fact]
        public void FromPactBroker_BearerAuth_AddsPactBrokerAuthArgs()
        {
            this.verifier.FromPactBroker(new Uri("http://broker.example.org/"), new PactUriOptions("token"));

            this.CheckArgs("--broker-url", "http://broker.example.org/",
                           "--token", "token");
        }

        [Fact]
        public void FromPactBroker_EnablePending_AddsPactBrokerPendingArgs()
        {
            this.verifier.FromPactBroker(new Uri("http://broker.example.org/"), enablePending: true);

            this.CheckArgs("--broker-url", "http://broker.example.org/",
                           "--enable-pending");
        }

        [Fact]
        public void FromPactBroker_ConsumerVersionTags_AddsPactBrokerConsumerVersionArgs()
        {
            this.verifier.FromPactBroker(new Uri("http://broker.example.org/"), consumerVersionTags: new[] { "v1", "v2" });

            this.CheckArgs("--broker-url", "http://broker.example.org/",
                           "--consumer-version-tags", "v1,v2");
        }

        [Fact]
        public void FromPactBroker_IncludeWipSince_AddsPactBrokerPendingArgs()
        {
            this.verifier.FromPactBroker(new Uri("http://broker.example.org/"), includeWipPactsSince: "5d");

            this.CheckArgs("--broker-url", "http://broker.example.org/",
                           "--include-wip-pacts-since", "5d");
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

        [Fact]
        public void WithPublishedResults_WhenCalled_AddsPublishArgs()
        {
            this.verifier.WithPublishedResults("1.2.3", new[] { "feature/branch", "production" });

            this.CheckArgs("--publish",
                           "--provider-version", "1.2.3",
                           "--provider-tags", "feature/branch,production");
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

        private void CheckArgs(params string[] args)
        {
            this.verifier.Verify();

            string formatted = string.Join(Environment.NewLine, args);

            this.mockProvider.Verify(v => v.Verify(formatted));
        }
    }
}
