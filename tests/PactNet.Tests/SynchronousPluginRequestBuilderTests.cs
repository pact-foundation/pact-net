using System.Collections.Generic;
using FluentAssertions;
using Moq;
using PactNet.Drivers.Plugins;
using Xunit;

namespace PactNet.Tests
{
    public class SynchronousPluginRequestBuilderTests
    {
        private readonly ISynchronousPluginRequestBuilderV4 builder;

        private readonly Mock<IPluginInteractionDriver> mockDriver;

        public SynchronousPluginRequestBuilderTests()
        {
            this.mockDriver = new Mock<IPluginInteractionDriver>();

            this.builder = new SynchronousPluginRequestBuilder(this.mockDriver.Object);
        }

        [Fact]
        public void Given_WhenCalled_AddsProviderState()
        {
            this.builder.Given("provider state");

            this.mockDriver.Verify(d => d.Given("provider state"));
        }

        [Fact]
        public void Given_WhenCalled_ReturnsSameBuilder()
        {
            ISynchronousPluginRequestBuilderV4 result = this.builder.Given("provider state");

            result.Should().BeSameAs(this.builder);
        }

        [Fact]
        public void Given_WithParam_AddsProviderState()
        {
            this.builder.Given("provider state", "foo", "bar");

            this.mockDriver.Verify(d => d.GivenWithParam("provider state", "foo", "bar"));
        }

        [Fact]
        public void Given_WithParam_ReturnsSameBuilder()
        {
            ISynchronousPluginRequestBuilderV4 result = this.builder.Given("provider state", "foo", "bar");

            result.Should().BeSameAs(this.builder);
        }

        [Fact]
        public void WithContent_WhenCalled_SetsInteractionContents()
        {
            var content = new Dictionary<string, object>
            {
                ["pact:proto"] = "greet.proto",
                ["pact:proto-service"] = "Greeter/SayHello",
                ["pact:content-type"] = "application/protobuf",
                ["request"] = new { name = "matching(type, 'foo')" },
                ["response"] = new { message = "matching(type, 'Hello foo')" }
            };

            this.builder.WithContent("application/grpc", content);

            this.mockDriver.Verify(d => d.WithContent("application/grpc", content), Times.Once);
        }
    }
}
