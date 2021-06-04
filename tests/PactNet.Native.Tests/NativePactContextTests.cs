using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using PactNet.Infrastructure.Outputters;
using Xunit;

namespace PactNet.Native.Tests
{
    public class NativePactContextTests
    {
        private readonly NativePactContext context;

        private readonly Mock<IMockServer> mockServer;
        private readonly Mock<IOutput> mockOutput;

        private readonly IFixture fixture;
        private readonly Uri serverUri;
        private readonly PactConfig config;

        public NativePactContextTests()
        {
            this.mockServer = new Mock<IMockServer>(MockBehavior.Strict);
            this.mockOutput = new Mock<IOutput>();

            this.fixture = new Fixture();
            this.serverUri = this.fixture.Create<Uri>();
            this.config = new PactConfig
            {
                Outputters = new[] { this.mockOutput.Object }
            };

            // set some default mock setups
            this.mockServer.Setup(s => s.MockServerLogs(this.serverUri.Port)).Returns(string.Empty);
            this.mockServer.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns(string.Empty);
            this.mockServer.Setup(s => s.WritePactFile(this.serverUri.Port, this.config.PactDir, false));
            this.mockServer.Setup(s => s.CleanupMockServer(this.serverUri.Port)).Returns(true);

            this.context = new NativePactContext(this.mockServer.Object, this.serverUri, this.config);
        }

        [Fact]
        public void Dispose_NoLogs_DoesNotWriteLogs()
        {
            this.context.Dispose();

            this.mockOutput.Verify(o => o.WriteLine(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Dispose_Logs_WritesLogsToOutput()
        {
            this.mockServer.Setup(s => s.MockServerLogs(this.serverUri.Port)).Returns("some logs");

            this.context.Dispose();

            this.mockOutput.Verify(o => o.WriteLine("some logs"));
        }

        [Fact]
        public void Dispose_NoMismatches_WritesPactFile()
        {
            this.context.Dispose();

            this.mockServer.Verify(s => s.WritePactFile(this.serverUri.Port, this.config.PactDir, false));
        }

        [Fact]
        public void Dispose_FailedToWritePactFile_ThrowsInvalidOperationException()
        {
            this.mockServer.Setup(s => s.WritePactFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws<InvalidOperationException>();

            Action action = () => this.context.Dispose();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Dispose_Mismatches_DoesNotWritePactFile()
        {
            this.mockServer.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns("some mismatches");

            try
            {
                this.context.Dispose();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            this.mockServer.Verify(s => s.WritePactFile(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public void Dispose_Mismatches_WritesMismatchesToOutput()
        {
            this.mockServer.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns("some mismatches");

            try
            {
                this.context.Dispose();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            this.mockOutput.Verify(o => o.WriteLine("some mismatches"));
        }

        [Fact]
        public void Dispose_Mismatches_ThrowsPactFailureException()
        {
            this.mockServer.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns("some mismatches");

            Action action = () => this.context.Dispose();

            action.Should().Throw<PactFailureException>();
        }

        [Fact]
        public void Dispose_Success_ShutsDownMockServer()
        {
            this.context.Dispose();

            this.mockServer.Verify(s => s.CleanupMockServer(this.serverUri.Port));
        }

        [Fact]
        public void Dispose_Failure_ShutsDownMockServer()
        {
            this.mockServer.Setup(s => s.MockServerMismatches(this.serverUri.Port)).Returns("some mismatches");

            try
            {
                this.context.Dispose();
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            this.mockServer.Verify(s => s.CleanupMockServer(this.serverUri.Port));
        }
    }
}
