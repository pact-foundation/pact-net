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
        private readonly Mock<IMessageMockServer> _mockedServer;

        public NativePactMessageBuilderTests()
        {
            _mockedServer = new Mock<IMessageMockServer>();
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
            //Arrange
            SetServerReifyMessage("{ \"param1\": \"value1\" }");

            void ExpectedDelegate(MessageModel e)
            {
            }

            var builder = new NativePactMessageBuilder(_mockedServer.Object, new MessagePactHandle(), null);

            //Act
            Action actualResult = () => builder.Verify<MessageModel>(ExpectedDelegate);

            //Assert
            actualResult.Should().Throw<PactMessageConsumerVerificationException>();
        }

        [Fact]
        public void Verify_Should_Fail_If_Verification_By_The_Consumer_Handler_Throws_Exception()
        {
            //Arrange
            var testMessage = new MessageModel(1, string.Empty).ToNativeMessage();

            SetServerReifyMessage(JsonConvert.SerializeObject(testMessage));

            void ExpectedDelegate(MessageModel e) => throw new Exception("exception test");
            
            var builder = new NativePactMessageBuilder(_mockedServer.Object, new MessagePactHandle(), null);

            //Act
            Action actualResult = () => builder.Verify<MessageModel>(ExpectedDelegate);

            //Assert
            actualResult.Should().Throw<PactMessageConsumerVerificationException>();
        }

        [Fact]
        public void Verify_Checks_Consumer_Handler_Completes_Successfully_With_Message()
        {
            //Arrange
            var testMessage = new MessageModel(1, "this message is a test").ToNativeMessage();

            SetServerReifyMessage(JsonConvert.SerializeObject(testMessage));

            void ExpectedDelegate(MessageModel e)
            {
            }

            var builder = new NativePactMessageBuilder(_mockedServer.Object, new MessagePactHandle(), null);

            //Act
            Action actualResult = () => builder.Verify<MessageModel>(ExpectedDelegate);

            //Assert
            actualResult.Should().Throw<PactMessageConsumerVerificationException>();
        }

        private class MessageModel
        {
            private readonly int _id;
            private readonly string _description;

            internal MessageModel(int id, string description)
            {
                _id = id;
                _description = description;
            }

            internal NativeMessage ToNativeMessage()
            {
                return new NativeMessage
                {
                    Contents = this
                };
            }
        }
    }
}
