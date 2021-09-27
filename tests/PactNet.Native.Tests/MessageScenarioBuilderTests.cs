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
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void WhenReceiving_Should_Fail_If_Wrong_Description(string description)
        {
            MessageScenarioBuilder.AScenario
                .Invoking(x => x.WhenReceiving(description))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenReceiving_Should_Add_Scenario()
        {
            var namedInteraction = "an interaction";

            var actualBuilder = MessageScenarioBuilder.AScenario.WhenReceiving(namedInteraction);

            actualBuilder.Should().NotBeNull();
        }
    }
}
