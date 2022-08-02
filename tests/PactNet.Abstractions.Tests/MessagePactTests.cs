using System;
using FluentAssertions;
using Xunit;
#pragma warning disable CS0618

namespace PactNet.Abstractions.Tests
{
    public class MessagePactTests
    {
        #region Support V3

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void V3_Handle_With_Invalid_Consumer_Throws_Exception(string consumer)
        {
            ((Action)(() => MessagePact.V3(consumer, "MyProvider"))).Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void V3_Handle_With_Invalid_Provider_Throws_Exception(string provider)
        {
            ((Action)(() => MessagePact.V3("MyConsumer", provider))).Should().Throw<ArgumentException>();
        }

        [Fact]
        public void V3_Handle_Has_Consumer_And_Provider()
        {
            var expectedConsumer = "MyConsumer";
            var expectedProvider = "MyProvider";

            var expectedConfig = new PactConfig();

            var pactMessage = MessagePact.V3(expectedConsumer, expectedProvider);

            pactMessage.Consumer.Should().Be(expectedConsumer);
            pactMessage.Provider.Should().Be(expectedProvider);
            pactMessage.Config.Should().BeEquivalentTo(expectedConfig);
        }

        [Fact]
        public void V3_Handle_Has_Consumer_And_Provider_And_Config()
        {
            var expectedConsumer = "MyConsumer";
            var expectedProvider = "MyProvider";

            var expectedConfig = new PactConfig();

            var pactMessage = MessagePact.V3(expectedConsumer, expectedProvider, expectedConfig);

            pactMessage.Consumer.Should().Be(expectedConsumer);
            pactMessage.Provider.Should().Be(expectedProvider);
            pactMessage.Config.Should().BeEquivalentTo(expectedConfig);
        }

        #endregion
    }
}
