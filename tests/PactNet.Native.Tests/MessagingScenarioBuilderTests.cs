using System;

using Xunit;

namespace PactNet.Native.Tests
{
    /// <summary>
    /// Defines the messaging scenario builder tests
    /// </summary>
    public class MessagingScenarioBuilderTests
    {
        private readonly IMessageScenarioBuilder _messageScenarioBuilder = MessageScenarioBuilder.Instance;

        [Fact]
        public void Instance_Property_Returns_Always_Same_Instance()
        {
            Assert.Equal(MessageScenarioBuilder.Instance, _messageScenarioBuilder);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void WhenReceiving_Adding_Invalid_Scenario_Throws_Exception(string namedInteraction)
        {
            Assert.Throws<ArgumentNullException>(() => _messageScenarioBuilder.WhenReceiving(namedInteraction));
        }

        [Fact]
        public void WhenReceiving_Adding_Scenario_Without_Action_Does_Not_Sets_Scenario()
        {
            var namedInteraction = "a wrong interaction";

            _messageScenarioBuilder.WhenReceiving(namedInteraction);

            Assert.Null(_messageScenarioBuilder.InvokeScenario(namedInteraction));
        }

        [Fact]
        public void WhenReceiving_Adding_Scenario_And_Action_Sets_Scenario()
        {
            var namedInteraction = "a named interaction";

            var expectedMessage = new { id = 1, description = "a description" };

            _messageScenarioBuilder.WhenReceiving(namedInteraction).WillPublishMessage(() => expectedMessage);

            var actualMessage = _messageScenarioBuilder.InvokeScenario(namedInteraction);

            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void WhenReceiving_Adding_Scenario_With_Same_Name_Throws_Exception()
        {
            var namedInteraction = "same name interaction";

            var expectedMessage = new { id = 1, description = "a description" };

            _messageScenarioBuilder.WhenReceiving(namedInteraction).WillPublishMessage(() => expectedMessage);

            Assert.Throws<InvalidOperationException>(() => _messageScenarioBuilder.WhenReceiving(namedInteraction));
        }

        [Fact]
        public void WhenReceiving_Twice_Throws_Exception()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _messageScenarioBuilder
                    .WhenReceiving("receiving first time")
                    .WhenReceiving("receiving second time"));
        }

        [Fact]
        public void WhenReceiving_Adding_Scenario_Action_Without_Description_Throws_Exception()
        {
            var expectedMessage = new { id = 1, description = "a description" };

            Assert.Throws<InvalidOperationException>(() => _messageScenarioBuilder.WillPublishMessage(() => expectedMessage));
        }
    }
}
