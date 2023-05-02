using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PactNet.Drivers;
using PactNet.Exceptions;
using PactNet.Interop;
using PactNet.Models;
using Xunit;

namespace PactNet.Tests
{
    public class ConfiguredMessageVerifierTests
    {
        private static readonly JsonSerializerSettings CamelCase = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private static readonly JsonSerializerSettings SnakeCase = new()
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        private readonly Mock<IMessageInteractionDriver> mockDriver;
        
        private readonly PactConfig config;

        public ConfiguredMessageVerifierTests()
        {
            this.mockDriver = new Mock<IMessageInteractionDriver>();
            
            this.config = new PactConfig { PactDir = "/path/to/pacts" };
        }

        [Theory]
        [InlineData(PactSpecification.V3)]
        [InlineData(PactSpecification.V4)]
        internal void Verify_SuccessfullyVerified_WritesPactFile(PactSpecification version)
        {
            (var verifier, Message contents) = this.SetupMessage(version);

            verifier.Verify<Message>(m => m.Should().BeEquivalentTo(contents));

            this.mockDriver.Verify(s => s.WritePactFile(this.config.PactDir));
        }

        [Theory]
        [InlineData(PactSpecification.V3)]
        [InlineData(PactSpecification.V4)]
        internal void Verify_MessageContentWithSnakeCasing_VerifiesSuccessfully(PactSpecification version)
        {
            (var verifier, Message contents) = this.SetupMessage(version, SnakeCase);

            verifier.Verify<Message>(m => m.Should().BeEquivalentTo(contents));
        }

        [Theory]
        [InlineData(PactSpecification.V3)]
        [InlineData(PactSpecification.V4)]
        internal void Verify_FailedToVerify_ThrowsVerificationException(PactSpecification version)
        {
            (var verifier, _) = this.SetupMessage(version);

            Action action = () => verifier.Verify<Message>(_ => throw new Exception("oh noes"));

            action.Should().Throw<PactMessageConsumerVerificationException>().WithInnerException<Exception>();
        }

        [Theory]
        [InlineData(PactSpecification.V3)]
        [InlineData(PactSpecification.V4)]
        internal void Verify_FailedToVerify_DoesNotWritePactFile(PactSpecification version)
        {
            (var verifier, _) = this.SetupMessage(version);

            try
            {
                verifier.Verify<Message>(_ => throw new Exception("oh noes"));
            }
            catch
            {
                // ignore
            }

            this.mockDriver.Verify(s => s.WritePactFile(It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData(PactSpecification.V3)]
        [InlineData(PactSpecification.V4)]
        internal async Task VerifyAsync_SuccessfullyVerified_WritesPactFile(PactSpecification version)
        {
            (var verifier, Message contents) = this.SetupMessage(version);

            await verifier.VerifyAsync<Message>(m =>
            {
                m.Should().BeEquivalentTo(contents);
                return Task.CompletedTask;
            });

            this.mockDriver.Verify(s => s.WritePactFile(this.config.PactDir));
        }

        [Theory]
        [InlineData(PactSpecification.V3)]
        [InlineData(PactSpecification.V4)]
        internal async Task VerifyAsync_FailedToVerifyAsync_ThrowsVerificationException(PactSpecification version)
        {
            (var verifier, _) = this.SetupMessage(version);

            Func<Task> action = () => verifier.VerifyAsync<Message>(_ => throw new Exception("oh noes"));

            await action.Should().ThrowAsync<PactMessageConsumerVerificationException>();
        }

        [Theory]
        [InlineData(PactSpecification.V3)]
        [InlineData(PactSpecification.V4)]
        internal async Task VerifyAsync_FailedToVerifyAsync_DoesNotWritePactFile(PactSpecification version)
        {
            (var verifier, _) = this.SetupMessage(version);

            try
            {
                await verifier.VerifyAsync<Message>(_ => throw new Exception("oh noes"));
            }
            catch
            {
                // ignore
            }

            this.mockDriver.Verify(s => s.WritePactFile(It.IsAny<string>()), Times.Never);
        }

        private (ConfiguredMessageVerifier Verifier, Message Message) SetupMessage(PactSpecification version, JsonSerializerSettings contentSettings = null)
        {
            var verifier = new ConfiguredMessageVerifier(this.mockDriver.Object, this.config, version);
            this.config.DefaultJsonSettings = contentSettings ?? CamelCase;

            // this simulates what the FFI library does - the content uses user-supplied JSON settings
            // then they are interpreted literally to a JToken
            var contents = new Message { FooBar = 42 };
            string serialised = JsonConvert.SerializeObject(contents, this.config.DefaultJsonSettings);

            JObject token = version switch
            {
                PactSpecification.V3 => JObject.Parse(serialised),
                PactSpecification.V4 => JObject.Parse(@$"{{""content"":{serialised},""contentType"":""application/json"",""encoded"":false}}"),
                _ => throw new ArgumentOutOfRangeException(nameof(version), version, "Unsupported version")
            };

            NativeMessage native = new NativeMessage
            {
                Description = "a message",
                Contents = token
            };

            // the native message returned from the FFI is always camel-cased
            this.mockDriver
                .Setup(s => s.Reify())
                .Returns(JsonConvert.SerializeObject(native, CamelCase));

            return (verifier, contents);
        }

        private class Message
        {
            public int FooBar { get; set; }
        }
    }
}
