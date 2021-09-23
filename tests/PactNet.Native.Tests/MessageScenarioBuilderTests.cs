using System;
using FluentAssertions;
using Xunit;

namespace PactNet.Native.Tests
{
    /// <summary>
    /// Defines the scenarios tests
    /// </summary>
    public class MessageScenarioBuilderTests
    {
        private readonly IMessageScenarioBuilder messageScenarioBuilder;

        public MessageScenarioBuilderTests()
        {
            this.messageScenarioBuilder = MessageScenarioBuilder.WillPublishMessage;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void WhenReceiving_Should_Fail_If_Wrong_Description(string description)
        {
            messageScenarioBuilder
                .Invoking(x => x.WhenReceiving(description))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenReceiving_Adding_Scenario_Without_Action_Does_Not_Sets_Scenario()
        {
            var namedInteraction = "a wrong interaction";

            var actualBuilder = this.messageScenarioBuilder.WhenReceiving(namedInteraction);

            actualBuilder.Should().NotBeNull();
        }
    }
}
