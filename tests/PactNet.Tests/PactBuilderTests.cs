using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Infrastructure.Outputters;
using PactNet.Interop;
using Xunit;

namespace PactNet.Tests
{
    public class PactBuilderTests
    {
        private readonly PactBuilder builder;

        private readonly Mock<ISynchronousHttpDriver> mockDriver;
        private readonly Mock<IOutput> mockOutput;

        private readonly IFixture fixture;
        private readonly PactHandle handle;
        private readonly Uri serverUri;
        private readonly PactConfig config;

        public PactBuilderTests()
        {
            this.mockDriver = new Mock<ISynchronousHttpDriver>(MockBehavior.Strict);
            this.mockOutput = new Mock<IOutput>();

            this.fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(this.fixture);

            this.handle = this.fixture.Create<PactHandle>();
            this.serverUri = this.fixture.Create<Uri>();
            this.config = new PactConfig
            {
                Outputters = new[] { this.mockOutput.Object }
            };

            // set some default mock setups
            this.mockDriver.Setup(s => s.CreateMockServerForPact(this.handle, "127.0.0.1:0", false)).Returns(this.serverUri.Port);
            this.mockDriver.Setup(s => s.NewHttpInteraction(this.handle, It.IsAny<string>())).Returns(new InteractionHandle());
            this.mockDriver.Setup(s => s.MockServerLogs(this.serverUri.Port)).Returns(string.Empty);
            this.mockDriver.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns(string.Empty);
            this.mockDriver.Setup(s => s.WritePactFile(this.handle, this.config.PactDir, false));
            this.mockDriver.Setup(s => s.CleanupMockServer(this.serverUri.Port)).Returns(true);

            this.builder = new PactBuilder(this.mockDriver.Object, this.handle, this.config);
        }

        [Fact]
        public void UponReceiving_WhenCalled_CreatesNewInteraction()
        {
            const string description = "test description";

            this.builder.UponReceiving(description);

            this.mockDriver.Verify(s => s.NewHttpInteraction(this.handle, description));
        }

        [Fact]
        public void Verify_WhenCalled_StartsMockServer()
        {
            this.builder.Verify(Success);

            this.mockDriver.Verify(s => s.CreateMockServerForPact(this.handle, "127.0.0.1:0", false));
        }

        [Fact]
        public void Verify_ErrorStartingMockServer_ThrowsInvalidOperationException()
        {
            this.mockDriver
                .Setup(s => s.CreateMockServerForPact(It.IsAny<PactHandle>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws<InvalidOperationException>();

            Action action = () => this.builder.Verify(Success);

            action.Should().Throw<InvalidOperationException>("because the mock server failed to start");
        }

        [Fact]
        public void Verify_Logs_WritesLogsToOutput()
        {
            const string expected = "some logs";
            this.mockDriver.Setup(s => s.MockServerLogs(this.serverUri.Port)).Returns(expected);

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

            this.mockDriver.Verify(s => s.WritePactFile(this.handle, this.config.PactDir, false));
        }

        [Fact]
        public void Verify_NoMismatches_ShutsDownMockServer()
        {
            this.builder.Verify(Success);

            this.mockDriver.Verify(s => s.CleanupMockServer(this.serverUri.Port));
        }

        [Fact]
        public void Verify_FailedToWritePactFile_ThrowsInvalidOperationException()
        {
            this.mockDriver
                .Setup(s => s.WritePactFile(It.IsAny<PactHandle>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws<InvalidOperationException>();

            Action action = () => this.builder.Verify(Success);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Verify_Mismatches_DoesNotWritePactFile()
        {
            this.mockDriver.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns("some mismatches");

            try
            {
                this.builder.Verify(Success);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            this.mockDriver.Verify(s => s.WritePactFile(It.IsAny<PactHandle>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void Verify_Mismatches_WritesMismatchesToOutput()
        {
            const string expected = "some mismatches";
            this.mockDriver.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns(expected);

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
            this.mockDriver.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns("some mismatches");

            Action action = () => this.builder.Verify(Success);

            action.Should().Throw<PactFailureException>();
        }

        [Fact]
        public void Verify_Mismatches_ShutsDownMockServer()
        {
            this.mockDriver.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns("some mismatches");

            try
            {
                this.builder.Verify(Success);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            this.mockDriver.Verify(s => s.CleanupMockServer(this.serverUri.Port));
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
