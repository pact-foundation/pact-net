using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using PactNet.Interop;
using System.Runtime.InteropServices;
using System.IO;
using PactNet;
using Xunit.Abstractions;

namespace GrpcGreeterClient.Tests
{
    public class GrpcGreeterClientTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public GrpcGreeterClientTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            PactLogLevel.Information.InitialiseLogging();
        }

        [Fact]
        public async Task ReturnsMismatchWhenNoGrpcClientRequestMade()
        {
            // arrange
            var host = "0.0.0.0";
            var pact = NativeInterop.NewPact("grpc-greeter-client", "grpc-greeter");
            var interaction = NativeInterop.NewSyncMessageInteraction(pact, "a request to a plugin");
            NativeInterop.WithSpecification(pact, PactSpecification.V4);
            var content = $@"{{
                    ""pact:proto"":""{Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GrpcGreeterClient", "Protos", "greet.proto").Replace("\\", "\\\\")}"",
                    ""pact:proto-service"": ""Greeter/SayHello"",
                    ""pact:content-type"": ""application/protobuf"",
                    ""request"": {{
                        ""name"": ""matching(type, 'foo')""
                    }},
                    ""response"": {{
                        ""message"": ""matching(type, 'Hello foo')""
                    }}
                }}";
            NativeInterop.PluginAdd(pact, "protobuf", "0.4.0");
            NativeInterop.PluginInteractionContents(interaction, 0, "application/grpc", content);

            var port = NativeInterop.CreateMockServerForTransport(pact, host, 0, "grpc", null);
            testOutputHelper.WriteLine("Port: " + port);

            var matched = NativeInterop.MockServerMatched(port);
            testOutputHelper.WriteLine("Matched: " + matched);
            matched.Should().BeFalse();

            var MismatchesPtr = NativeInterop.MockServerMismatches(port);
            var MismatchesString = Marshal.PtrToStringAnsi(MismatchesPtr);
            testOutputHelper.WriteLine("Mismatches: " + MismatchesString);
            var MismatchesJson = JsonSerializer.Deserialize<JsonElement>(MismatchesString);
            var ErrorString = MismatchesJson[0].GetProperty("error").GetString();
            var ExpectedPath = MismatchesJson[0].GetProperty("path").GetString();

            ErrorString.Should().Be("Did not receive any requests for path 'Greeter/SayHello'");
            ExpectedPath.Should().Be("Greeter/SayHello");

            NativeInterop.CleanupMockServer(port);
            NativeInterop.PluginCleanup(pact);
            await Task.Delay(1);
        }
        [Fact]
        public async Task WritesPactWhenGrpcClientRequestMade()
        {
            // arrange
            var host = "0.0.0.0";
            var pact = NativeInterop.NewPact("grpc-greeter-client", "grpc-greeter");
            var interaction = NativeInterop.NewSyncMessageInteraction(pact, "a request to a plugin");
            NativeInterop.WithSpecification(pact, PactSpecification.V4);
            var content = $@"{{
                    ""pact:proto"":""{Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "GrpcGreeterClient", "Protos", "greet.proto").Replace("\\", "\\\\")}"",
                    ""pact:proto-service"": ""Greeter/SayHello"",
                    ""pact:content-type"": ""application/protobuf"",
                    ""request"": {{
                        ""name"": ""matching(type, 'foo')""
                    }},
                    ""response"": {{
                        ""message"": ""matching(type, 'Hello foo')""
                    }}
                }}";

            NativeInterop.PluginAdd(pact, "protobuf", "0.4.0");
            NativeInterop.PluginInteractionContents(interaction, 0, "application/grpc", content);

            var port = NativeInterop.CreateMockServerForTransport(pact, host, 0, "grpc", null);
            testOutputHelper.WriteLine("Port: " + port);

            // act
            var client = new GreeterClientWrapper("http://localhost:" + port);
            var result = await client.SayHello("foo");
            testOutputHelper.WriteLine("Result: " + result);

            // assert
            result.Should().Be("Hello foo");
            var matched = NativeInterop.MockServerMatched(port);
            testOutputHelper.WriteLine("Matched: " + matched);
            matched.Should().BeTrue();

            var MismatchesPtr = NativeInterop.MockServerMismatches(port);
            var MismatchesString = Marshal.PtrToStringAnsi(MismatchesPtr);
            testOutputHelper.WriteLine("Mismatches: " + MismatchesString);

            MismatchesString.Should().Be("[]");

            var writeRes = NativeInterop.WritePactFileForPort(port, "../../../../pacts", false);
            testOutputHelper.WriteLine("WriteRes: " + writeRes);
            NativeInterop.CleanupMockServer(port);
            NativeInterop.PluginCleanup(pact);
        }

    }
}
