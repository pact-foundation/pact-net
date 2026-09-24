using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Interop;
using PactNet.Models;
using Xunit;

namespace PactNet.Tests
{
    public class ConfiguredSyncMessageVerifierTests
    {
        private static readonly JsonSerializerOptions CamelCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly Mock<ISyncMessageInteractionDriver> mockDriver;

        private readonly PactConfig config;

        public ConfiguredSyncMessageVerifierTests()
        {
            this.mockDriver = new Mock<ISyncMessageInteractionDriver>();

            this.config = new PactConfig { PactDir = "/path/to/pacts", DefaultJsonSettings = CamelCase };
        }

        [Fact]
        public void Verify_SuccessfullyVerified_WritesPactFile()
        {
            (var verifier, Request request) = this.SetupMessage();

            verifier.Verify<Request, Response>(r =>
            {
                r.Should().BeEquivalentTo(request);
                return new Response { Status = "ok" };
            });

            this.mockDriver.Verify(s => s.WritePactFile(this.config.PactDir));
        }

        [Fact]
        public void Verify_FailedToVerify_ThrowsVerificationException()
        {
            (var verifier, _) = this.SetupMessage();

            Action action = () => verifier.Verify<Request, Response>(_ => throw new Exception("oh noes"));

            action.Should().Throw<PactMessageConsumerVerificationException>().WithInnerException<Exception>();
        }

        [Fact]
        public void Verify_FailedToVerify_DoesNotWritePactFile()
        {
            (var verifier, _) = this.SetupMessage();

            try
            {
                verifier.Verify<Request, Response>(_ => throw new Exception("oh noes"));
            }
            catch
            {
                // ignore
            }

            this.mockDriver.Verify(s => s.WritePactFile(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task VerifyAsync_SuccessfullyVerified_WritesPactFile()
        {
            (var verifier, Request request) = this.SetupMessage();

            await verifier.VerifyAsync<Request, Response>(r =>
            {
                r.Should().BeEquivalentTo(request);
                return Task.FromResult(new Response { Status = "ok" });
            });

            this.mockDriver.Verify(s => s.WritePactFile(this.config.PactDir));
        }

        [Fact]
        public async Task VerifyAsync_FailedToVerifyAsync_ThrowsVerificationException()
        {
            (var verifier, _) = this.SetupMessage();

            Func<Task> action = () => verifier.VerifyAsync<Request, Response>(_ => throw new Exception("oh noes"));

            await action.Should().ThrowAsync<PactMessageConsumerVerificationException>();
        }

        private (ConfiguredSyncMessageVerifier Verifier, Request Request) SetupMessage()
        {
            var verifier = new ConfiguredSyncMessageVerifier(this.mockDriver.Object, this.config, PactSpecification.V4);

            // this simulates what the FFI library does - the content uses user-supplied JSON settings
            // then they are interpreted literally to a JToken
            var request = new Request { Id = 42 };
            string serialisedRequest = JsonSerializer.Serialize(request, this.config.DefaultJsonSettings);

            JsonNode requestToken = JsonSerializer.Deserialize<JsonNode>(
                @$"{{""content"":{serialisedRequest},""contentType"":""application/json"",""encoded"":false}}");

            var response = new Response { Status = "ok" };
            string serialisedResponse = JsonSerializer.Serialize(response, this.config.DefaultJsonSettings);
            JsonNode responseToken = JsonSerializer.Deserialize<JsonNode>(
                @$"{{""content"":{serialisedResponse},""contentType"":""application/json"",""encoded"":false}}");

            NativeSyncMessage native = new NativeSyncMessage
            {
                Request = new NativeMessage { Contents = requestToken },
                Response = new[] { new NativeMessage { Contents = responseToken } }
            };

            // the native message returned from the FFI is always camel-cased
            this.mockDriver
                .Setup(s => s.Reify())
                .Returns(JsonSerializer.Serialize(native, CamelCase));

            return (verifier, request);
        }

        private class Request
        {
            public int Id { get; set; }
        }

        private class Response
        {
            public string Status { get; set; }
        }
    }
}
