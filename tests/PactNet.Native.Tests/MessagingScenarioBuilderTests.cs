using System;

using FluentAssertions;

using Xunit;

using static PactNet.Native.MessageScenarioBuilder;

namespace PactNet.Native.Tests
{
    /// <summary>
    /// Defines the messaging scenario builder tests
    /// </summary>
    public class MessagingScenarioBuilderTests
    {
        private readonly IMessageScenarioBuilder _messageScenarioBuilder;

        public MessagingScenarioBuilderTests()
        {
            _messageScenarioBuilder = Scenario;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void WhenReceiving_Adding_Invalid_Scenario_Throws_Exception(string namedInteraction)
        {
            _messageScenarioBuilder
                .Invoking(x => x.WhenReceiving(namedInteraction))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WhenReceiving_Adding_Scenario_Without_Action_Does_Not_Sets_Scenario()
        {
            var namedInteraction = "a wrong interaction";

            _messageScenarioBuilder.WhenReceiving(namedInteraction);

            object actualMessage = _messageScenarioBuilder.InvokeScenario(namedInteraction);

            actualMessage.Should().BeNull();
        }

        [Fact]
        public void InvokeScenario_After_Setting_Scenario_Description_And_Action()
        {
            var namedInteraction = "a named interaction";

            var expectedMessage = new { id = 1, description = "a description" };

            _messageScenarioBuilder.WhenReceiving(namedInteraction).WillPublishMessage(() => expectedMessage);

            object actualMessage = _messageScenarioBuilder.InvokeScenario(namedInteraction);

            actualMessage.Should().BeEquivalentTo(expectedMessage);
        }

        [Fact]
        public void WhenReceiving_Adding_Scenario_With_Same_Name_Throws_Exception()
        {
            var namedInteraction = "same name interaction";

            var expectedMessage = new { id = 1, description = "a description" };

            _messageScenarioBuilder.WhenReceiving(namedInteraction).WillPublishMessage(() => expectedMessage);

            _messageScenarioBuilder
                .Invoking(x => x.WhenReceiving(namedInteraction))
                .Should().Throw<InvalidOperationException>($"Scenario called \"{namedInteraction}\" has already been added");
        }

        [Fact]
        public void WhenReceiving_Twice_Throws_Exception()
        {
            _messageScenarioBuilder
                .Invoking(x => x.WhenReceiving("receiving first time").WhenReceiving("receiving second time"))
                .Should().Throw<InvalidOperationException>("You need to set the scenario action before adding another scenario");
        }

        [Fact]
        public void WhenReceiving_Adding_Scenario_Action_Without_Description_Throws_Exception()
        {
            var expectedMessage = new { id = 1, description = "a description" };

            _messageScenarioBuilder
                .Invoking(x => x.WillPublishMessage(() => expectedMessage))
                .Should().Throw<InvalidOperationException>("You need to set the scenario description before the action");
        }

        [Fact]
        public void InvokeScenario_With_Wrong_Description_Returns_Null()
        {
            var namedInteraction = "a new interaction";

            var expectedMessage = new { id = 1, description = "a description" };

            _messageScenarioBuilder.WhenReceiving(namedInteraction).WillPublishMessage(() => expectedMessage);

            object actualMessage = _messageScenarioBuilder.InvokeScenario("another interaction");

            actualMessage.Should().BeNull();
        }
    }
}
