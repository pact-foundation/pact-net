using System;
using FluentAssertions;
using Moq;
using PactNet.Drivers;
using PactNet.Interop;
using Xunit;

namespace PactNet.Tests
{
    public class SyncMessagePactBuilderTests
    {
        private readonly SyncMessagePactBuilder builder;

        private readonly Mock<IMessagePactDriver> mockDriver;
        private readonly Mock<ISyncMessageInteractionDriver> mockInteractions;

        private readonly PactConfig config;

        public SyncMessagePactBuilderTests()
        {
            this.mockDriver = new Mock<IMessagePactDriver>();
            this.mockInteractions = new Mock<ISyncMessageInteractionDriver>();

            this.config = new PactConfig();

            this.mockDriver
                .Setup(d => d.NewSyncMessageInteraction(It.IsAny<string>()))
                .Returns(this.mockInteractions.Object);

            this.builder = new SyncMessagePactBuilder(this.mockDriver.Object, this.config, PactSpecification.V4);
        }

        [Fact]
        public void Ctor_NullDriver_ThrowsArgumentNullException()
        {
            Action action = () =>
            {
                var _ = new SyncMessagePactBuilder(null, this.config, PactSpecification.V4);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullConfig_ThrowsArgumentNullException()
        {
            Action action = () =>
            {
                var _ = new SyncMessagePactBuilder(this.mockDriver.Object, null, PactSpecification.V4);
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ExpectsToReceive_WhenCalled_StartsNewMessage()
        {
            this.builder.ExpectsToReceive("a message");

            this.mockDriver.Verify(s => s.NewSyncMessageInteraction("a message"), Times.Once);
        }

        [Fact]
        public void WithPactMetadata_WhenCalled_ConfiguresMetadata()
        {
            this.builder.WithPactMetadata("test", "name", "value");

            this.mockDriver.Verify(s => s.WithMessagePactMetadata("test", "name", "value"));
        }

        [Fact]
        public void WithPactMetadata_WhenCalled_ReturnsFluentBuilder()
        {
            var returned = this.builder.WithPactMetadata("test", "name", "value");

            returned.Should().BeSameAs(this.builder);
        }
    }
}
