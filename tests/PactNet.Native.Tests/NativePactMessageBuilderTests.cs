using System;

using FluentAssertions;

using Moq;

using Newtonsoft.Json;

using PactNet.Native.Exceptions;
using PactNet.Native.Models;

using Xunit;

namespace PactNet.Native.Tests
{
    public class NativePactMessageBuilderTests
    {
        private readonly Mock<IMockServer> _mockedServer;

        public NativePactMessageBuilderTests()
        {
            _mockedServer = new Mock<IMockServer>();
        }

        private void SetServerReifyMessage(string message)
        {
            _mockedServer
                .Setup(x => x.MessageReify(It.IsAny<MessageHandle>()))
                .Returns(message);
        }

        [Fact]
        public void Verify_Should_Fail_If_Type_Is_Not_The_Same_As_The_Message()
        {
            SetServerReifyMessage("{ \"param1\": \"value1\" }");

            Action<MessageModel> expectedDelegate = (MessageModel e) => { return; };
            var builder = new NativePactMessageBuilder(_mockedServer.Object, new MessagePactHandle(), null);

            Action actualResult = () => builder.Verify<MessageModel>(expectedDelegate);

            actualResult.Should().Throw<PactMessageConsumerVerificationException>();
        }

        [Fact]
        public void Verify_Should_Fail_If_Verification_By_The_Consumer_Handler_Throws_Exception()
        {
            var testMessage = new NativeMessage
            {
                Contents = new MessageModel
                {
                    Id = 1,
                    Description = "this message is a test"
                }
            };

            SetServerReifyMessage(JsonConvert.SerializeObject(testMessage));

            Action<MessageModel> expectedDelegate = (MessageModel e) => throw new Exception("exception test");
            var builder = new NativePactMessageBuilder(_mockedServer.Object, new MessagePactHandle(), null);

            Action actualResult = () => builder.Verify<MessageModel>(expectedDelegate);

            actualResult.Should().Throw<PactMessageConsumerVerificationException>();
        }

        [Fact]
        public void Verify_Checks_Consumer_Handler_Completes_Successfully_With_Message()
        {
            var testMessage = new NativeMessage
            {
                Contents = new MessageModel
                {
                    Id = 1,
                    Description = "this message is a test"
                }
            };

            SetServerReifyMessage(JsonConvert.SerializeObject(testMessage));

            Action<MessageModel> expectedDelegate = (MessageModel e) => { return; };
            var builder = new NativePactMessageBuilder(_mockedServer.Object, new MessagePactHandle(), null);

            Action actualResult = () => builder.Verify<MessageModel>(expectedDelegate);

            actualResult.Should().Throw<PactMessageConsumerVerificationException>();
        }

        internal class MessageModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}
