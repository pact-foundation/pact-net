using System;
using FluentAssertions.Extensions;
using Moq;
using Newtonsoft.Json;
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

            this.verifier = new PactVerifier(this.mockProvider.Object,
                                             this.mockMessaging.Object,
                                             new PactVerifierConfig
                                             {
                                                 Outputters = new[]
                                                 {
                                                     new XunitOutput(output)
                                                 }
                                             });
        }

        [Fact]
        public void ServiceProvider_WhenCalled_SetsProviderArgs()
        {
            this.verifier.ServiceProvider("provider name", new Uri("http://example.org:8080/base/path"));

            this.mockProvider.Verify(p => p.SetProviderInfo("provider name", "http", "example.org", 8080, "/base/path"));
        }

        [Fact]
        public void MessagingProvider_WhenCalled_SetsProviderInfo()
        {
            var settings = new JsonSerializerSettings();
            this.mockMessaging
                .Setup(m => m.Start(settings))
                .Returns(new Uri("https://localhost:1234/pact-messaging/"));

            this.verifier.MessagingProvider("My Producer", settings);

            this.mockProvider.Verify(p => p.SetProviderInfo("My Producer", "https", "localhost", 1234, "/pact-messaging/"));
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
                this.mockProvider
                    .Setup(p => p.Initialise())
                    .Verifiable();

                this.mockProvider
                    .Setup(p => p.SetProviderInfo("Provider", "http", "example.org", 8080, "/base-path"))
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
                    .ServiceProvider("Provider", new Uri("http://example.org:8080/base-path"))
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
