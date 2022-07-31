using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PactNet.Drivers;
using PactNet.Exceptions;
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

        private readonly ConfiguredMessageVerifier verifier;

        private readonly Mock<IMessageInteractionDriver> mockDriver;
        
        private readonly PactConfig config;

        public ConfiguredMessageVerifierTests()
        {
            this.mockDriver = new Mock<IMessageInteractionDriver>();
            
            this.config = new PactConfig { PactDir = "/path/to/pacts" };

            this.verifier = new ConfiguredMessageVerifier(this.mockDriver.Object, this.config);
        }

        [Fact]
        public void Verify_SuccessfullyVerified_WritesPactFile()
        {
            Message contents = this.SetupMessage();

            this.verifier.Verify<Message>(m => m.Should().BeEquivalentTo(contents));

            this.mockDriver.Verify(s => s.WritePactFile(this.config.PactDir));
        }

        [Fact]
        public void Verify_MessageContentWithSnakeCasing_VerifiesSuccessfully()
        {
            var contents = this.SetupMessage(SnakeCase);

            this.verifier.Verify<Message>(m => m.Should().BeEquivalentTo(contents));
        }

        [Fact]
        public void Verify_FailedToVerify_ThrowsVerificationException()
        {
            this.SetupMessage();

            Action action = () => this.verifier.Verify<Message>(_ => throw new Exception("oh noes"));

            action.Should().Throw<PactMessageConsumerVerificationException>().WithInnerException<Exception>();
        }

        [Fact]
        public void Verify_FailedToVerify_DoesNotWritePactFile()
        {
            this.SetupMessage();

            try
            {
                this.verifier.Verify<Message>(_ => throw new Exception("oh noes"));
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
            Message contents = this.SetupMessage();

            await this.verifier.VerifyAsync<Message>(m =>
            {
                m.Should().BeEquivalentTo(contents);
                return Task.CompletedTask;
            });

            this.mockDriver.Verify(s => s.WritePactFile(this.config.PactDir));
        }

        [Fact]
        public async Task VerifyAsync_FailedToVerifyAsync_ThrowsVerificationException()
        {
            this.SetupMessage();

            Func<Task> action = () => this.verifier.VerifyAsync<Message>(_ => throw new Exception("oh noes"));

            await action.Should().ThrowAsync<PactMessageConsumerVerificationException>();
        }

        [Fact]
        public async Task VerifyAsync_FailedToVerifyAsync_DoesNotWritePactFile()
        {
            this.SetupMessage();

            try
            {
                await this.verifier.VerifyAsync<Message>(_ => throw new Exception("oh noes"));
            }
            catch
            {
                // ignore
            }

            this.mockDriver.Verify(s => s.WritePactFile(It.IsAny<string>()), Times.Never);
        }

        private Message SetupMessage(JsonSerializerSettings contentSettings = null)
        {
            this.config.DefaultJsonSettings = contentSettings ?? CamelCase;

            // this simulates what the FFI library does - the content uses user-supplied JSON settings
            // then they are interpreted literally to a JToken
            var contents = new Message { FooBar = 42 };
            string serialised = JsonConvert.SerializeObject(contents, this.config.DefaultJsonSettings);
            JObject token = JObject.Parse(serialised);

            NativeMessage native = new NativeMessage
            {
                Description = "a message",
                Contents = token
            };

            // the native message returned from the FFI is always camel-cased
            this.mockDriver
                .Setup(s => s.Reify())
                .Returns(JsonConvert.SerializeObject(native, CamelCase));

            return contents;
        }

        private class Message
        {
            public int FooBar { get; set; }
        }
    }
}
