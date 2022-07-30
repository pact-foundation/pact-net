using System;
using FluentAssertions;
using Moq;
using PactNet.Drivers;
using PactNet.Interop;
using Xunit;

namespace PactNet.Tests
{
    public class MessagePactBuilderTests
    {
        private readonly MessagePactBuilder builder;

        private readonly Mock<IAsynchronousMessageDriver> mockDriver;

        private readonly PactHandle handle;
        private readonly PactConfig config;

        public MessagePactBuilderTests()
        {
            this.mockDriver = new Mock<IAsynchronousMessageDriver>();

            this.handle = new PactHandle();
            this.config = new PactConfig();

            this.builder = new MessagePactBuilder(this.mockDriver.Object, this.handle, this.config);
        }

        [Fact]
        public void Ctor_NullServer_ThrowsArgumentNullException()
        {
            Action action = () =>
            {
                var _ = new MessagePactBuilder(null, this.handle, this.config);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullConfig_ThrowsArgumentNullException()
        {
            Action action = () =>
            {
                var _ = new MessagePactBuilder(this.mockDriver.Object, this.handle, null);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ExpectsToReceive_WhenCalled_StartsNewMessage()
        {
            var message = new InteractionHandle();

            this.mockDriver
                .Setup(s => s.NewMessageInteraction(this.handle, "a message"))
                .Returns(message);

            this.builder.ExpectsToReceive("a message");

            this.mockDriver.Verify(s => s.ExpectsToReceive(message, "a message"), Times.Once);
        }

        [Fact]
        public void WithPactMetadata_WhenCalled_ConfiguresMetadata()
        {
            this.builder.WithPactMetadata("test", "name", "value");

            this.mockDriver.Verify(s => s.WithMessagePactMetadata(this.handle, "test", "name", "value"));
        }

        [Fact]
        public void WithPactMetadata_WhenCalled_ReturnsFluentBuilder()
        {
            var returned = this.builder.WithPactMetadata("test", "name", "value");

            returned.Should().BeSameAs(this.builder);
        }
    }
}
