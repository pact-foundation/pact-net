using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using Xunit;

namespace PactNet.Native.Tests
{
    public class NativePactBuilderTests
    {
        private readonly NativePactBuilder builder;

        private readonly Mock<IHttpMockServer> mockServer;

        private readonly IFixture fixture;
        private readonly PactHandle handle;
        private readonly PactConfig config;

        public NativePactBuilderTests()
        {
            this.mockServer = new Mock<IHttpMockServer>();

            this.fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(this.fixture);

            this.handle = this.fixture.Create<PactHandle>();
            this.config = new PactConfig();

            this.builder = new NativePactBuilder(this.mockServer.Object, this.handle, this.config);
        }

        [Fact]
        public void UponReceiving_WhenCalled_CreatesNewInteraction()
        {
            this.builder.UponReceiving("test description");

            this.mockServer.Verify(s => s.NewInteraction(this.handle, "test description"));
        }

        [Fact]
        public void Build_WhenCalled_StartsMockServer()
        {
            this.builder.Build();

            this.mockServer.Verify(s => s.CreateMockServerForPact(this.handle, "127.0.0.1:0", false));
        }

        [Fact]
        public void Build_ErrorStartingMockServer_ThrowsInvalidOperationException()
        {
            this.mockServer.Setup(
                    s => s.CreateMockServerForPact(It.IsAny<PactHandle>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws<InvalidOperationException>();

            Action action = () => this.builder.Build();

            action.Should().Throw<InvalidOperationException>("because the mock server failed to start");
        }
    }
}
