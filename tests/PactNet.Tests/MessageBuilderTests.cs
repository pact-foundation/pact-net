using System;
using System.Collections.Generic;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Drivers;
using PactNet.Interop;
using Xunit;

namespace PactNet.Tests
{
    public class MessageBuilderTests
    {
        private readonly IMessageBuilderV3 builder;

        private readonly Mock<IMessageInteractionDriver> mockDriver;
        
        private readonly PactConfig config;

        public MessageBuilderTests()
        {
            this.mockDriver = new Mock<IMessageInteractionDriver>();

            this.config = new PactConfig { DefaultJsonSettings = new JsonSerializerSettings() };

            this.builder = new MessageBuilder(this.mockDriver.Object, this.config, PactSpecification.V4);
        }

        [Fact]
        public void Ctor_Throws_Exception_If_Server_Not_Set()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new MessagePactBuilder(null, new PactConfig(), PactSpecification.V4));
        }

        [Fact]
        public void Given_WhenCalled_AddsProviderState()
        {
            this.builder.Given("provider state");

            this.mockDriver.Verify(s => s.Given("provider state"));
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

            this.mockDriver.Verify(s => s.GivenWithParam("provider state", "foo", "bar"));
            this.mockDriver.Verify(s => s.GivenWithParam("provider state", "baz", "bash"));
        }

        [Fact]
        public void WithMetadata_WhenCalled_AddsMetadata()
        {
            var expectedKey = "poolId";
            var expectedValue = "1234";

            this.builder.WithMetadata(expectedKey, expectedValue);

            this.mockDriver.Verify(s => s.WithMetadata(expectedKey, expectedValue));
        }

        [Fact]
        public void WithJsonContent_WithoutCustomSettings_AddsContentWithDefaultSettings()
        {
            var content = new { Id = 1, Desc = "description" };
            const string expected = @"{""Id"":1,""Desc"":""description""}";

            this.builder.WithJsonContent(content);

            this.mockDriver.Verify(s => s.WithContents("application/json", expected, 0));
        }

        [Fact]
        public void WithJsonContent_WithCustomSettings_AddsContentWithOverriddenSettings()
        {
            var content = new { Id = 1, Desc = "description" };
            const string expected = @"{""id"":1,""desc"":""description""}";

            this.builder.WithJsonContent(content, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            this.mockDriver.Verify(s => s.WithContents("application/json", expected, 0));
        }
    }
}
