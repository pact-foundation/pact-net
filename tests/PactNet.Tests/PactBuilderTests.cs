using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Infrastructure.Outputters;
using Xunit;

namespace PactNet.Tests
{
    public class PactBuilderTests
    {
        private readonly PactBuilder builder;

        private readonly Mock<IHttpPactDriver> mockDriver;
        private readonly Mock<IHttpInteractionDriver> mockInteractions;
        private readonly Mock<IMockServerDriver> mockServer;
        private readonly Mock<IOutput> mockOutput;

        private readonly IFixture fixture;
        private readonly Uri serverUri;
        private readonly PactConfig config;

        public PactBuilderTests()
        {
            this.mockDriver = new Mock<IHttpPactDriver>(MockBehavior.Strict);
            this.mockInteractions = new Mock<IHttpInteractionDriver>(MockBehavior.Strict);
            this.mockServer = new Mock<IMockServerDriver>(MockBehavior.Strict);
            this.mockOutput = new Mock<IOutput>();

            this.fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(this.fixture);
            
            this.serverUri = this.fixture.Create<Uri>();
            this.config = new PactConfig
            {
                Outputters = new[] { this.mockOutput.Object }
            };

            // set some default mock setups
            this.mockDriver.Setup(s => s.CreateMockServer("127.0.0.1", null, false)).Returns(this.mockServer.Object);
            this.mockDriver.Setup(s => s.NewHttpInteraction(It.IsAny<string>())).Returns(this.mockInteractions.Object);
            this.mockDriver.Setup(s => s.WritePactFile(this.config.PactDir));

            this.mockServer.Setup(s => s.Uri).Returns(this.serverUri);
            this.mockServer.Setup(s => s.MockServerLogs()).Returns(string.Empty);
            this.mockServer.Setup(s => s.MockServerMismatches()).Returns(string.Empty);
            this.mockServer.Setup(s => s.Dispose());

            this.builder = new PactBuilder(this.mockDriver.Object, this.config);
        }

        [Fact]
        public void UponReceiving_WhenCalled_CreatesNewInteraction()
        {
            const string description = "test description";

            this.builder.UponReceiving(description);

            this.mockDriver.Verify(d => d.NewHttpInteraction(description));
        }

        [Fact]
        public void Verify_WhenCalled_StartsMockServer()
        {
            this.builder.Verify(Success);

            this.mockDriver.Verify(d => d.CreateMockServer("127.0.0.1", null, false));
        }

        [Fact]
        public void Verify_ErrorStartingMockServer_ThrowsInvalidOperationException()
        {
            this.mockDriver
                .Setup(s => s.CreateMockServer(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<bool>()))
                .Throws<InvalidOperationException>();

            Action action = () => this.builder.Verify(Success);

            action.Should().Throw<InvalidOperationException>("because the mock server failed to start");
        }

        [Fact]
        public void Verify_Logs_WritesLogsToOutput()
        {
            const string expected = "some logs";
            this.mockServer.Setup(s => s.MockServerLogs()).Returns(expected);

            this.builder.Verify(Success);

            this.mockOutput.Verify(o => o.WriteLine(expected));
        }

        [Fact]
        public void Verify_InteractionThrowsException_RethrowsInteractionException()
        {
            this.builder
                .Invoking(b => b.Verify(Error))
                .Should().ThrowExactly<Exception>("because the inner interaction call failed");
        }

        [Fact]
        public void Verify_NoMismatches_WritesPactFile()
        {
            this.builder.Verify(Success);

            this.mockDriver.Verify(s => s.WritePactFile(this.config.PactDir));
        }

        [Fact]
        public void Verify_NoMismatches_ShutsDownMockServer()
        {
            this.builder.Verify(Success);

            this.mockServer.Verify(s => s.Dispose());
        }

        [Fact]
        public void Verify_FailedToWritePactFile_ThrowsInvalidOperationException()
        {
            this.mockDriver
                .Setup(s => s.WritePactFile(It.IsAny<string>()))
                .Throws<InvalidOperationException>();

            Action action = () => this.builder.Verify(Success);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Verify_Mismatches_DoesNotWritePactFile()
        {
            this.mockServer.Setup(s => s.MockServerMismatches()).Returns("some mismatches");

            try
            {
                this.builder.Verify(Success);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            this.mockDriver.Verify(s => s.WritePactFile(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Verify_Mismatches_WritesMismatchesToOutput()
        {
            const string expected = "some mismatches";
            this.mockServer.Setup(s => s.MockServerMismatches()).Returns(expected);

            try
            {
                this.builder.Verify(Success);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            this.mockOutput.Verify(o => o.WriteLine(expected));
        }

        [Fact]
        public void Verify_Mismatches_ThrowsPactFailureException()
        {
            this.mockServer.Setup(s => s.MockServerMismatches()).Returns("some mismatches");

            Action action = () => this.builder.Verify(Success);

            action.Should().Throw<PactFailureException>();
        }

        [Fact]
        public void Verify_Mismatches_ShutsDownMockServer()
        {
            this.mockServer.Setup(s => s.MockServerMismatches()).Returns("some mismatches");

            try
            {
                this.builder.Verify(Success);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            this.mockServer.Verify(s => s.Dispose());
        }

        private static void Success(IConsumerContext ctx)
        {
        }

        private static void Error(IConsumerContext ctx)
        {
            throw new Exception("Interaction failed");
        }
    }
}
