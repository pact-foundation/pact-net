using System;
using System.Collections.Generic;
using FluentAssertions.Extensions;
using Moq;
using PactNet.Verifier;
using Xunit;

namespace PactNet.Tests.Verifier
{
    public class PactBrokerOptionsTests
    {
        private static readonly Uri BrokerUri = new Uri("https://broker.example.org");

        private readonly PactBrokerOptions options;

        private readonly Mock<IVerifierProvider> mockProvider;

        public PactBrokerOptionsTests()
        {
            this.mockProvider = new Mock<IVerifierProvider>();

            this.options = new PactBrokerOptions(this.mockProvider.Object, BrokerUri);
        }

        [Fact]
        public void BasicAuthentication_WhenCalled_AddsAuthArgs()
        {
            this.options.BasicAuthentication("user@example.org", "hunter2");

            this.Verify(username: "user@example.org", password: "hunter2");
        }

        [Fact]
        public void TokenAuthentication_WhenCalled_AddsAuthArgs()
        {
            this.options.TokenAuthentication("abcde123");

            this.Verify(token: "abcde123");
        }

        [Fact]
        public void EnablePending_WhenCalled_AddsPactBrokerPendingArgs()
        {
            this.options.EnablePending();

            this.Verify(enablePending: true);
        }

        [Fact]
        public void ConsumerTags_WhenCalled_AddsPactBrokerConsumerVersionArgs()
        {
            this.options.ConsumerTags("v1", "v2");

            this.Verify(consumerVersionTags: new[] { "v1", "v2" });
        }

        [Fact]
        public void ConsumerVersionSelectors_WhenCalled_AddsConsumerVersionSelectorsArgs()
        {
            string[] expected =
            {
                @"{""mainBranch"":true,""matchingBranch"":true}",
                @"{""branch"":""feat/foo"",""fallbackBranch"":""main""}",
                @"{""tag"":""foo"",""fallbackTag"":""bar"",""latest"":false}",
                @"{""deployed"":true,""released"":true,""environment"":""prod""}",
                @"{""deployedOrReleased"":true,""consumer"":""My Consumer""}"
            };

            this.options.ConsumerVersionSelectors(new ConsumerVersionSelector { MainBranch = true, MatchingBranch = true },
                                                  new ConsumerVersionSelector { Branch = "feat/foo", FallbackBranch = "main" },
                                                  new ConsumerVersionSelector { Tag = "foo", FallbackTag = "bar", Latest = false },
                                                  new ConsumerVersionSelector { Released = true, Deployed = true, Environment = "prod" },
                                                  new ConsumerVersionSelector { DeployedOrReleased = true, Consumer = "My Consumer" });

            this.Verify(consumerVersionSelectors: expected);
        }

        [Fact]
        public void FromPactBroker_IncludeWipSince_AddsPactBrokerPendingArgs()
        {
            this.options.IncludeWipPactsSince(14.February(2021));

            this.Verify(wipSince: 14.February(2021));
        }

        [Fact]
        public void PublishResults_WhenCalled_AddsPublishArgs()
        {
            this.options.PublishResults("1.2.3", _ => { });
            this.options.Apply();

            this.mockProvider.Verify(p => p.SetPublishOptions("1.2.3",
                                                              It.IsAny<Uri>(),
                                                              It.IsAny<ICollection<string>>(),
                                                              It.IsAny<string>()));
        }

        private void Verify(string username = null,
                            string password = null,
                            string token = null,
                            bool enablePending = false,
                            DateTime? wipSince = null,
                            ICollection<string> providerTags = null,
                            string providerBranch = null,
                            ICollection<string> consumerVersionSelectors = null,
                            ICollection<string> consumerVersionTags = null)
        {
            this.options.Apply();

            this.mockProvider.Verify(p => p.AddBrokerSource(BrokerUri,
                                                            username,
                                                            password,
                                                            token,
                                                            enablePending,
                                                            wipSince,
                                                            providerTags ?? Array.Empty<string>(),
                                                            providerBranch,
                                                            consumerVersionSelectors ?? Array.Empty<string>(),
                                                            consumerVersionTags ?? Array.Empty<string>()));
        }
    }
}
