using System;
using FluentAssertions;
using Moq;
using PactNet.Drivers;
using PactNet.Drivers.Plugins;
using PactNet.Infrastructure.Outputters;
using PactNet.Models;
using Xunit;

namespace PactNet.Tests
{
    public class SynchronousPluginPactBuilderTests
    {
        private const int MockServerPort = 5000;
        private readonly SynchronousPluginPactBuilder builder;

        private readonly Mock<IPluginPactDriver> mockDriver;
        private readonly Mock<IPluginInteractionDriver> mockInteractions;
        private readonly Mock<IMockServerDriver> mockServer;
        private readonly Mock<IOutput> mockOutput;

        private readonly PactConfig config;

        public SynchronousPluginPactBuilderTests()
        {
            this.mockDriver = new Mock<IPluginPactDriver>(MockBehavior.Strict);
            this.mockInteractions = new Mock<IPluginInteractionDriver>(MockBehavior.Strict);
            this.mockServer = new Mock<IMockServerDriver>(MockBehavior.Strict);
            this.mockServer.Setup(s => s.Port).Returns(MockServerPort);
            this.mockOutput = new Mock<IOutput>();

            this.config = new PactConfig
            {
                Outputters = new[] { this.mockOutput.Object }
            };

            this.mockDriver.Setup(s => s.CreateMockServer("127.0.0.1", null, false, "grpc")).Returns(this.mockServer.Object);
            this.mockDriver.Setup(s => s.NewSyncInteraction(It.IsAny<string>())).Returns(this.mockInteractions.Object);
            this.mockDriver.Setup(s => s.WritePactFile(MockServerPort, this.config.PactDir));
            this.mockDriver.Setup(s => s.Dispose());

            this.mockServer.Setup(s => s.Uri).Returns(new Uri("http://127.0.0.1:5000"));
            this.mockServer.Setup(s => s.MockServerLogs()).Returns(string.Empty);
            this.mockServer.Setup(s => s.MockServerMismatches()).Returns(string.Empty);
            this.mockServer.Setup(s => s.Dispose());

            this.builder = new SynchronousPluginPactBuilder(this.mockDriver.Object, this.config, null, IPAddress.Loopback, "grpc");
        }

        [Fact]
        public void UponReceiving_WhenCalled_CreatesNewSyncInteraction()
        {
            this.builder.UponReceiving("a plugin interaction");

            this.mockDriver.Verify(d => d.NewSyncInteraction("a plugin interaction"));
        }

        [Fact]
        public void UponReceiving_CalledTwice_CreatesTwoInteractions()
        {
            this.builder.UponReceiving("first interaction");
            this.builder.UponReceiving("second interaction");

            this.mockDriver.Verify(d => d.NewSyncInteraction("first interaction"), Times.Once);
            this.mockDriver.Verify(d => d.NewSyncInteraction("second interaction"), Times.Once);
        }

        [Fact]
        public void Verify_WhenCalled_StartsMockServerWithPluginTransport()
        {
            this.builder.Verify(_ => { });

            this.mockDriver.Verify(d => d.CreateMockServer("127.0.0.1", null, false, "grpc"));
        }

        [Fact]
        public void Verify_NoMismatches_WritesPactFile()
        {
            this.builder.Verify(_ => { });

            this.mockDriver.Verify(d => d.WritePactFile(MockServerPort, this.config.PactDir));
        }

        [Fact]
        public void Verify_WhenCalled_PassesMockServerUriToConsumer()
        {
            Uri actual = null;

            this.builder.Verify(ctx => actual = ctx.MockServerUri);

            actual.Should().Be(new Uri("http://127.0.0.1:5000"));
        }

        [Fact]
        public void Dispose_WhenCalled_DisposesPact()
        {
            this.builder.Dispose();

            this.mockDriver.Verify(d => d.Dispose());
        }
    }
}
