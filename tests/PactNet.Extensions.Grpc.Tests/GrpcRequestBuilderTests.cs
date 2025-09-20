using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using PactNet.Interop;
using Xunit;
using Xunit.Abstractions;

namespace PactNet.Extensions.Grpc.Tests;

public class GrpcRequestBuilderTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void ConfiguresRequestAndResponse()
    {
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var builder = new GrpcRequestBuilder(new InteractionHandle());
        string protoFilePath = Path.Combine("..", "..", "..", "..", "GrpcGreeterClient", "Protos", "greet.proto");
        string serviceName = "Greeter";
        string methodName = "SayHello";
        var content = $@"{{
                    ""pact:proto"":""{protoFilePath.Replace("\\", "\\\\")}"",
                    ""pact:proto-service"": ""{serviceName}/{methodName}"",
                    ""pact:content-type"": ""application/protobuf"",
                    ""request"": {{
                        ""name"": ""matching(type, 'foo')""
                    }},
                    ""response"": {{
                        ""message"": ""matching(type, 'Hello foo')""
                    }}
                }}".Replace(Environment.NewLine, "").Replace("'", "\\u0027");


        builder.WithRequest(protoFilePath, serviceName, methodName, new { name = "matching(type, 'foo')" })
            .WillRespond().WithBody(new { message = "matching(type, 'Hello foo')" });
        var actual = JsonSerializer.Serialize(builder.InteractionContents);
        testOutputHelper.WriteLine(actual);
        Assert.Equal(content, actual, ignoreAllWhiteSpace: true);
    }
}
