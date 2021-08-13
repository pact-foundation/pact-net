using System;
using System.Collections.Generic;

using AutoFixture;

using FluentAssertions;

using Moq;

using Newtonsoft.Json;

using PactNet.Exceptions;

using Xunit;

namespace PactNet.Native.Tests
{
    public class NativePactMessageBuilderTests
    {
        private readonly IMessageBuilderV3 _builder;
        private readonly IPactMessageBuilderV3 _pactMessage;
        private readonly Mock<IMessageMockServer> _mockedServer;
        private readonly MessageHandle _handle;
        private readonly string _pactDir;
        private readonly MessagePactHandle _pactHandle;

        public NativePactMessageBuilderTests()
        {
            var fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);

            _pactDir = "C:/";
            _pactHandle = new MessagePactHandle();
            _handle = fixture.Create<MessageHandle>();
            _mockedServer = new Mock<IMessageMockServer>();

            _mockedServer
                .Setup(x => x.NewMessage(It.IsAny<MessagePactHandle>(), It.IsAny<string>()))
                .Returns(_handle);

            var config = new PactConfig { PactDir = _pactDir };
            _pactMessage = new NativePactMessageBuilder(_mockedServer.Object, _pactHandle, config);
            _builder = _pactMessage.ExpectsToReceive("a messaging interaction");
        }

        [Fact]
        public void Ctor_Throws_Exception_If_Server_Not_Set()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new NativePactMessageBuilder(null, new MessagePactHandle(), new PactConfig()));
        }

        [Fact]
        public void Ctor_Throws_Exception_If_Config_Not_Set()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new NativePactMessageBuilder(_mockedServer.Object, new MessagePactHandle(), null));
        }

        [Fact]
        public void ExpectsTeReceive_WhenCalled_AddsDescription()
        {
            _pactMessage.ExpectsToReceive("provider state");

            _mockedServer.Verify(s => s.ExpectsToReceive(_handle, "provider state"));
        }

        [Fact]
        public void Given_WhenCalled_AddsProviderState()
        {
            _builder.Given("provider state");

            _mockedServer.Verify(s => s.Given(_handle, "provider state"));
        }

        [Fact]
        public void Given_WithParams_AddsProviderState()
        {
            _builder.Given("provider state",
                new Dictionary<string, string>
                {
                    ["foo"] = "bar",
                    ["baz"] = "bash",
                });

            _mockedServer.Verify(s => s.GivenWithParam(_handle, "provider state", "foo", "bar"));
            _mockedServer.Verify(s => s.GivenWithParam(_handle, "provider state", "baz", "bash"));
        }

        [Fact]
        public void WithMetadata_WhenCalled_AddsMetadata()
        {
            var expectedKey = "poolId";
            var expectedValue = "1234";

            _builder.WithMetadata(expectedKey, expectedValue);

            _mockedServer.Verify(s => s.WithMetadata(_handle, expectedKey, expectedValue));
        }

        [Fact]
        public void WithContent_WhenCalled_AddsContent()
        {
            var content = new { id = 1, desc = "description" };
            _builder.WithContent(content);

            _mockedServer.Verify(s => s.WithContents(_handle, "application/json", JsonConvert.SerializeObject(content), 100));
        }

        [Fact]
        public void Verify_Should_Write_Pact_File()
        {
            //Arrange
            var content = new MessageModel { Id = 1, Description = "description" };
            _builder.WithContent(content);

            SetServerReifyMessage(JsonConvert.SerializeObject(content.ToNativeMessage()));

            //Act
            _pactMessage.Verify<MessageModel>(_ => { });

            _mockedServer.Verify(s => s.WriteMessagePactFile(_pactHandle, _pactDir, true));
        }

        [Fact]
        public void Verify_Should_Fail_If_Type_Is_Not_The_Same_As_The_Message()
        {
            //Arrange
            SetServerReifyMessage("{ \"param1\": \"value1\" }");

            //Act
            _pactMessage
                .Invoking(x => x.Verify<MessageModel>(_ => { }))
                .Should().Throw<PactMessageConsumerVerificationException>();
        }

        [Fact]
        public void Verify_Should_Fail_If_Verification_By_The_Consumer_Handler_Throws_Exception()
        {
            //Arrange
            var testMessage = new MessageModel(1, string.Empty).ToNativeMessage();

            SetServerReifyMessage(JsonConvert.SerializeObject(testMessage));

            //Act
            _pactMessage
                .Invoking(x => x.Verify<MessageModel>(_ => throw new Exception("an exception when running the consumer handler")))
                .Should().Throw<PactMessageConsumerVerificationException>();
        }

        private void SetServerReifyMessage(string message)
        {
            _mockedServer
                .Setup(x => x.Reify(It.IsAny<MessageHandle>()))
                .Returns(message);
        }
    }
}
