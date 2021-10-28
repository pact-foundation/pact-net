using FluentAssertions.Extensions;
using Moq;
using PactNet.Verifier;
using Xunit;

namespace PactNet.Tests.Verifier
{
    public class PactBrokerOptionsTests
    {
        private readonly PactBrokerOptions options;

        private readonly Mock<IVerifierArguments> verifierArgs;

        public PactBrokerOptionsTests()
        {
            this.verifierArgs = new Mock<IVerifierArguments>();

            this.options = new PactBrokerOptions(this.verifierArgs.Object);
        }

        [Fact]
        public void BasicAuthentication_WhenCalled_AddsAuthArgs()
        {
            this.options.BasicAuthentication("user@example.org", "hunter2");

            this.verifierArgs.Verify(a => a.AddOption("--user", "user@example.org", "username"));
            this.verifierArgs.Verify(a => a.AddOption("--password", "hunter2", "password"));
        }

        [Fact]
        public void TokenAuthentication_WhenCalled_AddsAuthArgs()
        {
            this.options.TokenAuthentication("abcde123");

            this.verifierArgs.Verify(a => a.AddOption("--token", "abcde123", "token"));
        }

        [Fact]
        public void EnablePending_WhenCalled_AddsPactBrokerPendingArgs()
        {
            this.options.EnablePending();

            this.verifierArgs.Verify(a => a.AddFlag("--enable-pending"));
        }

        [Fact]
        public void ConsumerTags_WhenCalled_AddsPactBrokerConsumerVersionArgs()
        {
            this.options.ConsumerTags("v1", "v2");
            
            this.verifierArgs.Verify(a => a.AddOption("--consumer-version-tags", "v1,v2", "tags"));
        }

        [Fact]
        public void ConsumerVersionSelectors_WhenCalled_AddsConsumerVersionSelectorsArgs()
        {
            string expected = string.Join(",",
                                          @"{""mainBranch"":true,""matchingBranch"":true}",
                                          @"{""branch"":""feat/foo"",""fallbackBranch"":""main""}",
                                          @"{""tag"":""foo"",""fallbackTag"":""bar"",""latest"":false}",
                                          @"{""deployed"":true,""released"":true,""environment"":""prod""}",
                                          @"{""deployedOrReleased"":true,""consumer"":""My Consumer""}");

            this.options.ConsumerVersionSelectors(new ConsumerVersionSelector { MainBranch = true, MatchingBranch = true },
                                                  new ConsumerVersionSelector { Branch = "feat/foo", FallbackBranch = "main" },
                                                  new ConsumerVersionSelector { Tag = "foo", FallbackTag = "bar", Latest = false },
                                                  new ConsumerVersionSelector { Released = true, Deployed = true, Environment = "prod" },
                                                  new ConsumerVersionSelector { DeployedOrReleased = true, Consumer = "My Consumer" });

            this.verifierArgs.Verify(a => a.AddOption("--consumer-version-selectors", $"[{expected}]", null));
        }

        [Fact]
        public void FromPactBroker_IncludeWipSince_AddsPactBrokerPendingArgs()
        {
            this.options.IncludeWipPactsSince(14.February(2021));
            
            this.verifierArgs.Verify(a => a.AddOption("--include-wip-pacts-since", "2021-02-14", "date"));
        }

        [Fact]
        public void PublishResults_WhenCalled_AddsPublishArgs()
        {
            this.options.PublishResults("1.2.3", "feature/branch", "production");

            this.verifierArgs.Verify(a => a.AddFlag("--publish"));
            this.verifierArgs.Verify(a => a.AddOption("--provider-version", "1.2.3", "providerVersion"));
            this.verifierArgs.Verify(a => a.AddOption("--provider-tags", "feature/branch,production", null));
        }
    }
}
