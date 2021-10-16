using System.Collections.Generic;
using FluentAssertions;
using FluentAssertions.Extensions;
using PactNet.Verifier;
using Xunit;

namespace PactNet.Tests.Verifier
{
    public class PactBrokerOptionsTests
    {
        private readonly PactBrokerOptions options;

        private readonly IDictionary<string, string> verifierArgs;

        public PactBrokerOptionsTests()
        {
            this.verifierArgs = new Dictionary<string, string>();

            this.options = new PactBrokerOptions(this.verifierArgs);
        }

        [Fact]
        public void BasicAuthentication_WhenCalled_AddsAuthArgs()
        {
            this.options.BasicAuthentication("username", "password");

            this.verifierArgs
                .Should().Contain("--user", "username")
                .And.Contain("--password", "password");
        }

        [Fact]
        public void TokenAuthentication_WhenCalled_AddsAuthArgs()
        {
            this.options.TokenAuthentication("token");

            this.verifierArgs.Should().Contain("--token", "token");
        }

        [Fact]
        public void EnablePending_WhenCalled_AddsPactBrokerPendingArgs()
        {
            this.options.EnablePending();

            this.verifierArgs.Should().Contain("--enable-pending", string.Empty);
        }

        [Fact]
        public void ConsumerTags_WhenCalled_AddsPactBrokerConsumerVersionArgs()
        {
            this.options.ConsumerTags("v1", "v2");

            this.verifierArgs.Should().Contain("--consumer-version-tags", "v1,v2");
        }

        [Fact]
        public void FromPactBroker_IncludeWipSince_AddsPactBrokerPendingArgs()
        {
            this.options.IncludeWipPactsSince(14.February(2021));

            this.verifierArgs.Should().Contain("--include-wip-pacts-since", "2021-02-14");
        }

        [Fact]
        public void PublishResults_WhenCalled_AddsPublishArgs()
        {
            this.options.PublishResults("1.2.3", "feature/branch", "production");

            this.verifierArgs
                .Should().Contain("--publish", string.Empty)
                .And.Contain("--provider-version", "1.2.3")
                .And.Contain("--provider-tags", "feature/branch,production");
        }
    }
}
