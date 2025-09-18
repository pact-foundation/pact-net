using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Extensions.Grpc.Tests;

public class GrpcRequestBuilderTests(ITestOutputHelper testOutputHelper)
{
    private readonly Mock<ISynchronousPluginRequestBuilderV4> mockRequestBuilder = new();

    private GrpcRequestBuilder CreateBuilder() => new(this.mockRequestBuilder.Object);

    [Fact]
    public void WithBody_WhenCalled_SendsCompleteContentsToPlugin()
    {
        var builder = this.CreateBuilder();
        string protoFilePath = Path.Combine("..", "..", "..", "..", "GrpcGreeterClient", "Protos", "greet.proto");
        string serviceName = "Greeter";
        string methodName = "SayHello";
        var expected = $@"{{
                    ""pact:proto"":""{protoFilePath.Replace("\\", "\\\\")}"",
                    ""pact:proto-service"": ""{serviceName}/{methodName}"",
                    ""pact:content-type"": ""application/protobuf"",
                    ""request"": {{
                        ""name"": ""matching(type, 'foo')""
                    }},
                    ""response"": {{
                        ""message"": ""matching(type, 'Hello foo')""
                    }}
                }}".Replace("\r", "").Replace("\n", "").Replace("'", "\\u0027");

        Dictionary<string, object> sent = null;
        this.mockRequestBuilder
            .Setup(b => b.WithContent("application/grpc", It.IsAny<Dictionary<string, object>>()))
            .Callback<string, Dictionary<string, object>>((_, content) => sent = content);

        builder.WithRequest(protoFilePath, serviceName, methodName, new { name = "matching(type, 'foo')" })
            .WillRespond().WithBody(new { message = "matching(type, 'Hello foo')" });

        this.mockRequestBuilder.Verify(b => b.WithContent("application/grpc", It.IsAny<Dictionary<string, object>>()), Times.Once);
        var actual = JsonSerializer.Serialize(sent);
        testOutputHelper.WriteLine(actual);
        Assert.Equal(expected, actual, ignoreAllWhiteSpace: true);
    }

    [Fact]
    public void WithRequest_WhenCalled_DoesNotSendContentsYet()
    {
        var builder = this.CreateBuilder();

        builder.WithRequest("greet.proto", "Greeter", "SayHello", new { name = "foo" });

        this.mockRequestBuilder.Verify(b => b.WithContent(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Never);
    }

    [Fact]
    public void WithRequest_CalledTwice_Throws()
    {
        var builder = this.CreateBuilder();
        builder.WithRequest("greet.proto", "Greeter", "SayHello", new { name = "foo" });

        Assert.Throws<InvalidOperationException>(() => builder.WithRequest("greet.proto", "Greeter", "SayHello", new { name = "bar" }));
    }

    [Fact]
    public void WillRespond_BeforeRequestConfigured_Throws()
    {
        var builder = this.CreateBuilder();

        Assert.Throws<InvalidOperationException>(() => builder.WillRespond());
    }

    [Fact]
    public void WithBody_CalledTwice_Throws()
    {
        var response = this.CreateBuilder()
            .WithRequest("greet.proto", "Greeter", "SayHello", new { name = "foo" })
            .WillRespond();
        response.WithBody(new { message = "Hello foo" });

        Assert.Throws<InvalidOperationException>(() => response.WithBody(new { message = "Hello again" }));
        this.mockRequestBuilder.Verify(b => b.WithContent(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once);
    }

    [Fact]
    public void Given_WhenCalled_AddsProviderState()
    {
        this.CreateBuilder().Given("provider state");

        this.mockRequestBuilder.Verify(b => b.Given("provider state"));
    }

    [Fact]
    public void Given_WithParam_AddsProviderState()
    {
        this.CreateBuilder().Given("provider state", "foo", "bar");

        this.mockRequestBuilder.Verify(b => b.Given("provider state", "foo", "bar"));
    }
}
