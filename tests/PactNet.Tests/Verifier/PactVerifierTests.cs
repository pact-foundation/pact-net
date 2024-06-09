using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Tests.Verifier
{
    public class PactVerifierTests
    {
        private readonly PactVerifier verifier;

        private readonly Mock<IVerifierProvider> mockProvider;
        private readonly Mock<IMessagingProvider> mockMessaging;

        public PactVerifierTests(ITestOutputHelper output)
        {
            this.mockProvider = new Mock<IVerifierProvider>();
            this.mockMessaging = new Mock<IMessagingProvider>();

            this.verifier = new PactVerifier("Provider",
                                             new PactVerifierConfig
                                             {
                                                 Outputters = new[]
                                                 {
                                                     new XunitOutput(output)
                                                 }
                                             },
                                             this.mockProvider.Object,
                                             this.mockMessaging.Object);
        }

        public class UnitTests : PactVerifierTests
        {
            public UnitTests(ITestOutputHelper output) : base(output)
            {
            }

            [Fact]
            public void WithHttpEndpoint_WhenCalled_SetsProviderArgs()
            {
                this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path"));

                this.mockProvider.Verify(p => p.SetProviderInfo("Provider", "http", "example.org", 8080, "/base/path"));
            }

            [Fact]
            public void WithHttpEndpoint_CalledTwice_Throws()
            {
                Action action = () => this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path")).WithHttpEndpoint(new Uri("http://example.org:8080/base/path"));

                action.Should().Throw<InvalidOperationException>();
            }

            [Fact]
            public void WithMessages_WhenCalled_SetsProviderInfo()
            {
                var settings = new JsonSerializerOptions();
                this.mockMessaging
                    .Setup(m => m.Start(settings))
                    .Returns(new Uri("https://localhost:1234/pact-messaging/"));

                this.verifier.WithMessages(_ => { }, settings);

                this.mockProvider.Verify(p => p.SetProviderInfo("Provider", "message", "localhost", 1234, "/pact-messaging/"));
            }

            [Fact]
            public void WithMessages_CalledTwice_Throws()
            {
                this.mockMessaging
                    .Setup(m => m.Start(It.IsAny<JsonSerializerOptions>()))
                    .Returns(new Uri("https://localhost:1234/pact-messaging/"));

                Action action = () => this.verifier.WithMessages(_ => { }).WithMessages(_ => { });

                action.Should().Throw<InvalidOperationException>();
            }

            [Fact]
            public void WithFileSource_WhenCalled_AddsFileArg()
            {
                var file = new FileInfo("/home/user/pacts/file.json");

                this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path")).WithFileSource(file);

                this.mockProvider.Verify(p => p.AddFileSource(file));
            }

            [Fact]
            public void WithDirectorySource_WhenCalled_AddsDirectoryArg()
            {
                var directory = new DirectoryInfo("/home/user/pacts/");

                this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path")).WithDirectorySource(directory);

                this.mockProvider.Verify(p => p.AddDirectorySource(directory));
            }

            [Fact]
            public void WithDirectorySource_WithConsumerFilters_AddsFilterArgs()
            {
                var directory = new DirectoryInfo("/home/user/pacts/");

                this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path")).WithDirectorySource(directory, "foo", "bar");

                this.mockProvider.Verify(p => p.SetConsumerFilters(new[] { "foo", "bar" }));
            }

            [Fact]
            public void WithUriSource_NoOptions_AddsUrlArg()
            {
                var uri = new Uri("http://example.org/pact/file.json");

                this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path")).WithUriSource(uri);

                this.mockProvider.Verify(p => p.AddUrlSource(uri, null, null, null));
            }

            [Fact]
            public void WithUriSource_WithOptions_AddsUrlSource()
            {
                var uri = new Uri("http://example.org.pact.file.json");

                this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path"))
                    .WithUriSource(uri,
                                   options => options.BasicAuthentication("username", "password")
                                                     .TokenAuthentication("token"));

                this.mockProvider.Verify(p => p.AddUrlSource(uri, "username", "password", "token"));
            }

            [Fact]
            public void WithUriSource_NullOptions_AddsUrlSource()
            {
                var uri = new Uri("http://example.org/pact/file.json");

                this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path")).WithUriSource(uri, null);

                this.mockProvider.Verify(p => p.AddUrlSource(uri, null, null, null));
            }

            [Fact]
            public void WithPactBrokerSource_WhenCalled_AddsPactBrokerArg()
            {
                var brokerUri = new Uri("http://broker.example.org/");

                this.verifier.WithHttpEndpoint(new Uri("http://example.org:8080/base/path")).WithPactBrokerSource(brokerUri);

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

        public class IntegrationTests : PactVerifierTests
        {
            public IntegrationTests(ITestOutputHelper output) : base(output)
            {
            }

            [Fact]
            public void PactBrokerSource()
            {
                // arrange
                this.mockMessaging
                    .Setup(m => m.Start(It.IsAny<JsonSerializerOptions>()))
                    .Returns(new Uri("https://localhost:1234/pact-messaging/"));

                this.mockMessaging
                    .Setup(m => m.Scenarios)
                    .Returns(Mock.Of<IMessageScenarios>);

                this.mockProvider
                    .Setup(p => p.Initialise())
                    .Verifiable();

                this.mockProvider
                    .Setup(p => p.SetProviderInfo("Provider", "http", "example.org", 8080, "/base-path"))
                    .Verifiable();

                this.mockProvider
                    .Setup(p => p.AddTransport("message", 1234, "/pact-messaging/", null))
                    .Verifiable();

                this.mockProvider
                    .Setup(p => p.AddBrokerSource(new Uri("https://broker.example.org"),
                                                  "user",
                                                  "hunter2",
                                                  null,
                                                  true,
                                                  14.February(2022),
                                                  new[] { "provider-tag1", "provider-tag2" },
                                                  "provider-branch",
                                                  new[]
                                                  {
                                                      "{\"deployedOrReleased\":true}",
                                                      "{\"mainBranch\":true,\"latest\":true}"
                                                  },
                                                  new[] { "consumer-tag1", "consumer-tag2" }))
                    .Verifiable();

                this.mockProvider
                    .Setup(p => p.SetPublishOptions("1.2.3+abcd123",
                                                    new Uri("https://ci.example.org/builds/1234"),
                                                    new[] { "publish-tag1", "publish-tag2" },
                                                    "provider-publish-branch"))
                    .Verifiable();

                this.mockProvider
                    .Setup(p => p.SetProviderState(new Uri("http://example.org:8080/provider-states"), false, true))
                    .Verifiable();

                this.mockProvider
                    .Setup(p => p.SetFilterInfo("description", "state", null))
                    .Verifiable();

                this.mockProvider
                    .Setup(p => p.SetVerificationOptions(true, 10.Seconds()))
                    .Verifiable();

                // act
                this.verifier
                    .WithHttpEndpoint(new Uri("http://example.org:8080/base-path"))
                    .WithMessages(messages =>
                    {
                        messages.Add("a message", () => new { Foo = "bar" });
                    })
                    .WithPactBrokerSource(new Uri("https://broker.example.org"), options =>
                     {
                         options.BasicAuthentication("user", "hunter2")
                                .ConsumerTags("consumer-tag1", "consumer-tag2")
                                .ConsumerVersionSelectors(new ConsumerVersionSelector { DeployedOrReleased = true },
                                                          new ConsumerVersionSelector { MainBranch = true, Latest = true })
                                .EnablePending()
                                .IncludeWipPactsSince(14.February(2022))
                                .ProviderBranch("provider-branch")
                                .ProviderTags("provider-tag1", "provider-tag2")
                                .PublishResults("1.2.3+abcd123", publish =>
                                {
                                    publish.BuildUri(new Uri("https://ci.example.org/builds/1234"))
                                           .ProviderBranch("provider-publish-branch")
                                           .ProviderTags("publish-tag1", "publish-tag2");
                                });
                     })
                    .WithProviderStateUrl(new Uri("http://example.org:8080/provider-states"))
                    .WithFilter("description", "state")
                    .WithRequestTimeout(10.Seconds())
                    .WithSslVerificationDisabled()
                    .Verify();

                // assert
                this.mockProvider.Verify();
            }
        }
    }
}
