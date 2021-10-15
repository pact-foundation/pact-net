using System;
using FluentAssertions;
using PactNet.Verifier.Messaging;
using Xunit;

namespace PactNet.Abstractions.Tests.Verifier.Messaging
{
    /// <summary>
    /// Defines the scenarios tests
    /// </summary>
    public class MessageScenarioContentBuilderTests
    {
        /// <summary>
        /// The builder under test
        /// </summary>
        private readonly MessageScenarioContentBuilder scenarioContentBuilder;

        public MessageScenarioContentBuilderTests()
        {
            this.scenarioContentBuilder = new MessageScenarioContentBuilder("a good description");
        }

        [Fact]
        public void Should_Be_Able_To_Add_Metadata()
        {
            dynamic expectedMetadata = new { key = "value"};

            this.scenarioContentBuilder.WithMetadata(expectedMetadata);

            var actualScenario = this.scenarioContentBuilder.WithContent(new { field = "value" });

            Assert.Equal(expectedMetadata, actualScenario.Metadata);
        }

        [Fact]
        public void Should_Fail_To_Add_Metadata_If_Invalid_Metadata()
        {
            this.scenarioContentBuilder
                .Invoking(x => x.WithMetadata(null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Allow_Scenario_Invoking_After_Setting_Content_With_Method()
        {
            object expectedContent = new { field = "value"};

            var actualScenario = this.scenarioContentBuilder.WithContent(() => expectedContent);

            object actualContent = actualScenario.InvokeScenario();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void Should_Fail_Setting_Content_With_Invalid_Method()
        {
            this.scenarioContentBuilder
                .Invoking(x => x.WithContent(action: null))
                .Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Allow_Scenario_Invoking_After_Setting_Content_With_Dynamic_Object()
        {
            object expectedContent = new { field = "value"};

            var actualScenario = this.scenarioContentBuilder.WithContent(expectedContent);

            object actualContent = actualScenario.InvokeScenario();

            Assert.Equal(expectedContent, actualContent);
        }

        [Fact]
        public void Should_Fail_Setting_Content_With_Invalid_Dynamic_Object()
        {
            this.scenarioContentBuilder
                .Invoking(x => x.WithContent(messageContent: null))
                .Should().Throw<ArgumentNullException>();
        }
    }
}
