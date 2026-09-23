using System;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace PactNet.Extensions.Grpc.Tests;

public class GrpcPactBuilderTests
{
    private readonly Mock<ISynchronousPluginPactBuilderV4> mockPactBuilder = new();
    private readonly GrpcPactBuilder builder;

    public GrpcPactBuilderTests()
    {
        this.mockPactBuilder
            .Setup(p => p.UponReceiving(It.IsAny<string>()))
            .Returns(() => new Mock<ISynchronousPluginRequestBuilderV4>().Object);

        this.builder = new GrpcPactBuilder(this.mockPactBuilder.Object);
    }

    [Fact]
    public void UponReceiving_WhenCalled_CreatesNewInteraction()
    {
        this.builder.UponReceiving("a greeting request");

        this.mockPactBuilder.Verify(p => p.UponReceiving("a greeting request"), Times.Once);
    }

    [Fact]
    public void UponReceiving_CalledTwice_CreatesTwoInteractions()
    {
        this.builder.UponReceiving("first greeting");
        this.builder.UponReceiving("second greeting");

        this.mockPactBuilder.Verify(p => p.UponReceiving("first greeting"), Times.Once);
        this.mockPactBuilder.Verify(p => p.UponReceiving("second greeting"), Times.Once);
    }

    [Fact]
    public void Verify_WhenCalled_DelegatesToPluginPactBuilder()
    {
        Action<IConsumerContext> interact = _ => { };

        this.builder.Verify(interact);

        this.mockPactBuilder.Verify(p => p.Verify(interact), Times.Once);
    }

    [Fact]
    public async Task VerifyAsync_WhenCalled_DelegatesToPluginPactBuilder()
    {
        Func<IConsumerContext, Task> interact = _ => Task.CompletedTask;
        this.mockPactBuilder.Setup(p => p.VerifyAsync(interact)).Returns(Task.CompletedTask);

        await this.builder.VerifyAsync(interact);

        this.mockPactBuilder.Verify(p => p.VerifyAsync(interact), Times.Once);
    }

    [Fact]
    public void Dispose_WhenCalled_DisposesPluginPactBuilder()
    {
        this.builder.Dispose();

        this.mockPactBuilder.Verify(p => p.Dispose(), Times.Once);
    }
}
