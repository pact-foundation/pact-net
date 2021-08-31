using System;
using System.Collections.Generic;
using AutoFixture;
using Moq;
using Newtonsoft.Json;
using PactNet.Native.Interop;
using Xunit;

namespace PactNet.Native.Tests
{
    public class NativeMessageBuilderTests
    {
        private readonly IMessageBuilderV3 builder;
        private readonly Mock<IMessageMockServer> mockedServer;
        private readonly MessageHandle handle;

        public NativeMessageBuilderTests()
        {
            var fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(fixture);
            
            this.handle = fixture.Create<MessageHandle>();
            this.mockedServer = new Mock<IMessageMockServer>();

            this.builder = new NativeMessageBuilder(this.mockedServer.Object, this.handle);
        }

        [Fact]
        public void Ctor_Throws_Exception_If_Server_Not_Set()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new NativeMessagePactBuilder(null, new MessagePactHandle(), new PactConfig()));
        }

        [Fact]
        public void Given_WhenCalled_AddsProviderState()
        {
            this.builder.Given("provider state");

            this.mockedServer.Verify(s => s.Given(this.handle, "provider state"));
        }

        [Fact]
        public void Given_WithParams_AddsProviderState()
        {
            this.builder.Given("provider state",
                new Dictionary<string, string>
                {
                    ["foo"] = "bar",
                    ["baz"] = "bash",
                });

            this.mockedServer.Verify(s => s.GivenWithParam(this.handle, "provider state", "foo", "bar"));
            this.mockedServer.Verify(s => s.GivenWithParam(this.handle, "provider state", "baz", "bash"));
        }

        [Fact]
        public void WithMetadata_WhenCalled_AddsMetadata()
        {
            var expectedKey = "poolId";
            var expectedValue = "1234";

            this.builder.WithMetadata(expectedKey, expectedValue);

            this.mockedServer.Verify(s => s.WithMetadata(this.handle, expectedKey, expectedValue));
        }

        [Fact]
        public void WithContent_WhenCalled_AddsContent()
        {
            var content = new { id = 1, desc = "description" };
            this.builder.WithContent(content);

            this.mockedServer.Verify(s => s.WithContents(this.handle, "application/json", JsonConvert.SerializeObject(content), 100));
        }
    }
}
