using System;
using System.IO;
using FluentAssertions;
using FluentAssertions.Extensions;
using PactNet.Exceptions;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class InteropVerifierProviderTests
    {
        private readonly ITestOutputHelper output;

        public InteropVerifierProviderTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void HappyPathIntegrationTest()
        {
            using var provider = new InteropVerifierProvider(new PactVerifierConfig
            {
                LogLevel = PactLogLevel.Trace,
                Outputters = new[] { new XUnitOutput(this.output) }
            });

            provider.Initialise();
            provider.SetProviderInfo("integration-test", "http", "localhost", 12684, "/path");
            provider.SetProviderState(new Uri("http://localhost:12684/provider-state"), false, true);
            provider.SetConsumerFilters(new[] { "consumer" });
            provider.SetVerificationOptions(false, TimeSpan.FromMilliseconds(100));
            provider.SetPublishOptions("1.2.3", new Uri("https://ci.example.org/builds/12345"), new[] { "tags" }, "branch");
            provider.SetFilterInfo("description", "state", false);
            provider.AddCustomHeader("Authorization", "Bearer abcdef1234567890");

            provider.AddFileSource(new FileInfo("data/v2-consumer-integration.json"));
            provider.AddDirectorySource(new DirectoryInfo("data"));
            provider.AddUrlSource(new Uri("http://example.org/file.json"), "user", "pass", "token");
            provider.AddBrokerSource(new Uri("http://broker.example.org"),
                                     "user",
                                     "pass",
                                     "token",
                                     false,
                                     14.February(2021),
                                     new[] { "tag" },
                                     "main",
                                     new[] { @"{""branch"":""main""}" },
                                     new[] { "consumer-tag" });

            Action action = () => provider.Execute();

            action.Should().Throw<PactFailureException>();
        }

        [Fact]
        [Trait("Issue", "https://github.com/pact-foundation/pact-net/issues/376")]
        public void SetPublishOptions_NoBuildUri_IsValid()
        {
            using var provider = new InteropVerifierProvider(new PactVerifierConfig
            {
                LogLevel = PactLogLevel.Trace,
                Outputters = new[] { new XUnitOutput(this.output) }
            });

            provider.Initialise();
            provider.SetProviderInfo("integration-test", "http", "localhost", 12684, "/path");
            provider.SetPublishOptions("1.2.3", null, new[] { "tags" }, "branch");
            provider.AddFileSource(new FileInfo("data/v2-consumer-integration.json"));

            Action action = () => provider.Execute();

            action.Should().Throw<PactFailureException>();
        }
    }
}
