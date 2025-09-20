using System;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using System.IO;
using System.Text.Json.Serialization;
using PactNet;
using PactNet.Exceptions;
using PactNet.Extensions.Grpc;
using PactNet.Output.Xunit;
using Xunit.Abstractions;

namespace GrpcGreeterClient.Tests
{
    public class GrpcGreeterClientTests : IDisposable
    {
        private readonly IGrpcPactBuilderV4 pact;

        public GrpcGreeterClientTests(ITestOutputHelper output)
        {
            var config = new PactConfig
            {
                PactDir = "../../../../pacts/",
                Outputters = new[]
                {
                    new XunitOutput(output)
                },
                DefaultJsonSettings = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                },
                LogLevel = PactLogLevel.Debug
            };

            this.pact = Pact.V4("grpc-greeter-client", "grpc-greeter", config).WithGrpcInteractions();
        }

        [Fact]
        public void ThrowsExceptionWhenNoGrpcClientRequestMade()
        {
            string protoFilePath = Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GrpcGreeterClient", "Protos", "greet.proto");
            this.pact
                .UponReceiving("A greeting request to say hello.")
                .WithRequest(protoFilePath, nameof(Greeter), "SayHello",
                    new { name = "matching(type, 'foo')" })
                .WillRespond()
                .WithBody(new { message = "matching(type, 'Hello foo')" });

            Assert.Throws<PactFailureException>(() =>
                this.pact.Verify(ctx =>
                {
                    // No grpc call here results in failure.
                }));
        }

        [Fact]
        public async Task WritesPactForGreeterSayHelloRequest()
        {
            // Arrange
            string protoFilePath = Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GrpcGreeterClient", "Protos", "greet.proto");
            this.pact
                .UponReceiving("A greeting request to say hello.")
                .WithRequest(protoFilePath, nameof(Greeter), "SayHello",
                    new { name = "matching(type, 'foo')" })
                .WillRespond()
                .WithBody(new { message = "matching(type, 'Hello foo')" });

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
