using System;
using System.IO;
using Moq;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierProviderTests
    {
        private readonly PactVerifierProvider verifier;

        private readonly Mock<IVerifierArguments> verifierArgs;

        public PactVerifierProviderTests(ITestOutputHelper output)
        {
            this.verifierArgs = new Mock<IVerifierArguments>();

            this.verifier = new PactVerifierProvider(this.verifierArgs.Object, new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            });
        }

        [Fact]
        public void WithFileSource_WhenCalled_AddsFileArg()
        {
            var file = new FileInfo("/home/user/pacts/file.json");

            this.verifier.WithFileSource(file);

            this.verifierArgs.Verify(a => a.AddOption("--file", file.FullName, null));
        }

        [Fact]
        public void WithDirectorySource_WhenCalled_AddsDirectoryArg()
        {
            var directory = new DirectoryInfo("/home/user/pacts/");

            this.verifier.WithDirectorySource(directory);

            this.verifierArgs.Verify(a => a.AddOption("--dir", directory.FullName, null));
        }

        [Fact]
        public void WithDirectorySource_WithConsumerFilters_AddsFilterArgs()
        {
            var directory = new DirectoryInfo("/home/user/pacts/");

            this.verifier.WithDirectorySource(directory, "foo", "bar");

            this.verifierArgs.Verify(a => a.AddOption("--filter-consumer", "foo", null));
            this.verifierArgs.Verify(a => a.AddOption("--filter-consumer", "bar", null));
        }

        [Fact]
        public void WithUriSource_WhenCalled_AddsUrlArg()
        {
            this.verifier.WithUriSource(new Uri("http://example.org/pact/file.json"));

            this.verifierArgs.Verify(a => a.AddOption("--url", "http://example.org/pact/file.json", null));
        }

        [Fact]
        public void WithPactBrokerSource_WhenCalled_AddsPactBrokerArg()
        {
            this.verifier.WithPactBrokerSource(new Uri("http://broker.example.org/"));

            this.verifierArgs.Verify(a => a.AddOption("--broker-url", "http://broker.example.org/", null));
        }
    }
}
