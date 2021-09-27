using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using PactNet.Exceptions;
using PactNet.Native.Interop;
using Xunit;

namespace PactNet.Native.Tests
{
    public class NativePactMessageBuilderTests
    {
        private readonly IMessagePactBuilderV3 messagePact;
        private readonly Mock<IMessageMockServer> mockedServer;
        private readonly MessagePactHandle pactHandle;

        public NativePactMessageBuilderTests()
        {
            var fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);

            this.pactHandle = fixture.Create<MessagePactHandle>();
            this.mockedServer = new Mock<IMessageMockServer>();

            this.mockedServer
                .Setup(x => x.NewMessage(It.IsAny<MessagePactHandle>(), It.IsAny<string>()))
                .Returns(fixture.Create<MessageHandle>());

            var config = new PactConfig { PactDir = "C:/" };
            this.messagePact = new NativeMessagePactBuilder(this.mockedServer.Object, this.pactHandle, config);
        }

        [Fact]
        public void Ctor_Should_Fail_If_Server_Not_Set()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new NativeMessagePactBuilder(null, new MessagePactHandle(), new PactConfig()));
        }

        [Fact]
        public void Ctor_Should_Fail_If_Config_Not_Set()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new NativeMessagePactBuilder(this.mockedServer.Object, new MessagePactHandle(), null));
        }

        [Fact]
        public void Should_Be_Able_To_Add_Description()
        {
            var expectedDescription = "description of the messaging interaction";
            this.messagePact.ExpectsToReceive(expectedDescription);

            this.mockedServer.Verify(s => s.NewMessage(this.pactHandle, It.IsAny<string>()));
            this.mockedServer.Verify(s => s.ExpectsToReceive(It.IsAny<MessageHandle>(), expectedDescription));
        }

        [Fact]
        public void Should_Be_Able_To_Add_Metadata()
        {
            string expectedValue = "value1";
            string expectedName = "parameter1";
            string expectedNamespace = "MyNamespace";

            this.messagePact.WithPactMetadata(expectedNamespace, expectedName, expectedValue);

            this.mockedServer.Verify(s => s.WithMessagePactMetadata(this.pactHandle, expectedNamespace, expectedName, expectedValue));
        }

        [Fact]
        public void Verify_Should_Write_Pact_File()
        {
            //Arrange
            var content = new MessageModel { Id = 1, Description = "description" };
            this.messagePact
                .ExpectsToReceive("a description")
                .WithJsonContent(content);

            SetServerReifyMessage(JsonConvert.SerializeObject(content.ToNativeMessage()));

            //Act
            this.messagePact.Verify<MessageModel>(_ => { });

            this.mockedServer.Verify(s => s.WriteMessagePactFile(It.IsAny<MessagePactHandle>(), It.IsAny<string>(), false));
        }

        [Fact]
        public void Verify_Should_Fail_If_Type_Is_Not_The_Same_As_The_Message()
        {
            //Arrange
            SetServerReifyMessage("{ \"param1\": \"value1\" }");

            //Act
            this.messagePact
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
            this.messagePact
                .Invoking(x => x.Verify<MessageModel>(_ => throw new Exception("an exception when running the consumer handler")))
                .Should().Throw<PactMessageConsumerVerificationException>();
        }

        [Fact]
        public async Task VerifyAsync_Should_Write_Pact_File()
        {
            //Arrange
            var content = new MessageModel { Id = 1, Description = "description" };
            this.messagePact
                .ExpectsToReceive("a description")
                .WithJsonContent(content);

            SetServerReifyMessage(JsonConvert.SerializeObject(content.ToNativeMessage()));

            //Act
            await this.messagePact.VerifyAsync<MessageModel>(_ => Task.CompletedTask);

            this.mockedServer.Verify(s => s.WriteMessagePactFile(It.IsAny<MessagePactHandle>(), It.IsAny<string>(), false));
        }

        [Fact]
        public async Task VerifyAsync_Should_Fail_If_Type_Is_Not_The_Same_As_The_Message()
        {
            //Arrange
            SetServerReifyMessage("{ \"param1\": \"value1\" }");

            Func<Task> actual = this.messagePact.Awaiting(x => x.VerifyAsync<MessageModel>(_ => Task.CompletedTask));

            await actual.Should().ThrowAsync<PactMessageConsumerVerificationException>();
        }

        [Fact]
        public async Task VerifyAsync_Should_Fail_If_Verification_By_The_Consumer_Handler_Throws_Exception()
        {
            //Arrange
            var testMessage = new MessageModel(1, string.Empty).ToNativeMessage();

            SetServerReifyMessage(JsonConvert.SerializeObject(testMessage));

            Func<Task> actual = this.messagePact.Awaiting(x => x.VerifyAsync<MessageModel>(_ => throw new Exception("an exception when running the consumer handler")));

            await actual.Should().ThrowAsync<PactMessageConsumerVerificationException>();
        }

        private void SetServerReifyMessage(string message)
        {
            this.mockedServer
                .Setup(x => x.Reify(It.IsAny<MessageHandle>()))
                .Returns(message);
        }
    }
}
