using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using PactNet.Verifier;
using PactNet.xUnit;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierProviderTests
    {
        private readonly PactVerifierProvider verifier;

        private readonly Mock<IVerifierProvider> mockProvider;

        public PactVerifierProviderTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            this.verifier = new PactVerifierProvider(this.mockProvider.Object, new PactVerifierConfig
            {
                Outputters = new[]
                {
                    output.AsPactOutput()
                }
            });
        }

        [Fact]
        public void WithFileSource_WhenCalled_AddsFileArg()
        {
            var file = new FileInfo("/home/user/pacts/file.json");

            this.verifier.WithFileSource(file);

            this.mockProvider.Verify(p => p.AddFileSource(file));
        }

        [Fact]
        public void WithDirectorySource_WhenCalled_AddsDirectoryArg()
        {
            var directory = new DirectoryInfo("/home/user/pacts/");

            this.verifier.WithDirectorySource(directory);

            this.mockProvider.Verify(p => p.AddDirectorySource(directory));
        }

        [Fact]
        public void WithDirectorySource_WithConsumerFilters_AddsFilterArgs()
        {
            var directory = new DirectoryInfo("/home/user/pacts/");

            this.verifier.WithDirectorySource(directory, "foo", "bar");

            this.mockProvider.Verify(p => p.SetConsumerFilters(new[] { "foo", "bar" }));
        }

        [Fact]
        public void WithUriSource_NoOptions_AddsUrlArg()
        {
            var uri = new Uri("http://example.org/pact/file.json");

            this.verifier.WithUriSource(uri);

            this.mockProvider.Verify(p => p.AddUrlSource(uri, null, null, null));
        }

        [Fact]
        public void WithUriSource_WithOptions_AddsUrlSource()
        {
            var uri = new Uri("http://example.org.pact.file.json");

            this.verifier.WithUriSource(uri, options => options.BasicAuthentication("username", "password")
                                                               .TokenAuthentication("token"));

            this.mockProvider.Verify(p => p.AddUrlSource(uri, "username", "password", "token"));
        }

        [Fact]
        public void WithUriSource_NullOptions_AddsUrlSource()
        {
            var uri = new Uri("http://example.org/pact/file.json");

            this.verifier.WithUriSource(uri, null);

            this.mockProvider.Verify(p => p.AddUrlSource(uri, null, null, null));
        }

        [Fact]
        public void WithPactBrokerSource_WhenCalled_AddsPactBrokerArg()
        {
            var brokerUri = new Uri("http://broker.example.org/");

            this.verifier.WithPactBrokerSource(brokerUri);

            this.mockProvider.Verify(p => p.AddBrokerSource(brokerUri,
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<bool>(),
                                                            It.IsAny<DateTime?>(),
                                                            It.IsAny<ICollection<string>>(),
                                                            It.IsAny<string>(),
                                                            It.IsAny<ICollection<string>>(),
                                                            It.IsAny<ICollection<string>>()));
        }
    }
}
