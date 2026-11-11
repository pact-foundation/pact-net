using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using System.IO;
using System.Text.Json.Serialization;
using PactNet;
using PactNet.Exceptions;
using PactNet.Output.Xunit;
using Xunit.Abstractions;

namespace GrpcGreeterClient.Tests
{
    public class GrpcGreeterClientTests : IDisposable
    {
        private readonly ISynchronousPluginPactBuilderV4 pact;

        public GrpcGreeterClientTests(ITestOutputHelper output)
        {
            var config = new PactConfig
            {
                PactDir = "../../../../pacts/",
                Outputters = new[] { new XunitOutput(output) },
                DefaultJsonSettings = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                },
                LogLevel = PactLogLevel.Information
            };

            this.pact = Pact.V4("grpc-greeter-client", "grpc-greeter", config)
                .WithSynchronousPluginInteractions("protobuf", "0.8.0", transport: "grpc");
        }

        [Fact]
        public void ThrowsExceptionWhenNoGrpcClientRequestMade()
        {
            string protoFilePath = Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GrpcGreeterClient", "Protos", "greet.proto");
            var content = new Dictionary<string, object>
            {
                {
                    "pact:proto", protoFilePath
                },
                { "pact:proto-service", "Greeter/SayHello" },
                { "pact:content-type", "application/protobuf" },
                { "request", new { name = "matching(equalTo, 'foo')" } },
                { "response", new { message = "matching(equalTo, 'Hello foo')" } }
            };

            this.pact.UponReceiving("A greeting request to say hello.").WithContent("application/grpc", content);

            this.pact.Invoking(p => p.Verify(_ => { })).Should().Throw<PactFailureException>();
        }

        [Fact]
        public async Task WritesPactForGreeterSayHelloRequest()
        {
            string protoFilePath = Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GrpcGreeterClient", "Protos", "greet.proto");
            var content = new Dictionary<string, object>
            {
                {
                    "pact:proto", protoFilePath
                },
                { "pact:proto-service", "Greeter/SayHello" },
                { "pact:content-type", "application/protobuf" },
                { "request", new { name = "matching(equalTo, 'foo')" } },
                { "response", new { message = "matching(equalTo, 'Hello foo')" } }
            };


            this.pact.UponReceiving("A greeting request to say hello.").WithContent("application/grpc", content);

            await this.pact.VerifyAsync(async ctx =>
            {

                // Arrange
                var client = new GreeterClientWrapper(ctx.MockServerUri.AbsoluteUri);

                // Act
                var greeting = await client.SayHello("foo");

                // Assert
                greeting.Should().Be("Hello foo");
            });
        }

        public void Dispose() => this.pact?.Dispose();
    }
}
