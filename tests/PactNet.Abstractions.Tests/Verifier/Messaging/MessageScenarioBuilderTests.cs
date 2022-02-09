using System;
using FluentAssertions;
using PactNet.Verifier.Messaging;
using Xunit;

namespace PactNet.Abstractions.Tests.Verifier.Messaging
{
    /// <summary>
    /// Defines the scenarios tests
    /// </summary>
    public class MessageScenarioBuilderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void WhenReceiving_Should_Fail_If_Wrong_Description(string description)
        {
            Action action = () => MessageScenarioBuilder.WhenReceiving(description);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenReceiving_Should_Add_Scenario()
        {
            var namedInteraction = "an interaction";

            var actualBuilder = MessageScenarioBuilder.WhenReceiving(namedInteraction);

            actualBuilder.Should().NotBeNull();
        }
    }
}
