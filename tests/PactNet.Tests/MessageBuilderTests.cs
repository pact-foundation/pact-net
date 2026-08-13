using System;
using System.Collections.Generic;
using System.Text.Json;
using Moq;
using PactNet.Drivers;
using PactNet.Interop;
using Xunit;
using Match = PactNet.Matchers.Match;

namespace PactNet.Tests
{
    public class MessageBuilderTests
    {
        private readonly IMessageBuilderV4 builder;

        private readonly Mock<IMessageInteractionDriver> mockDriver;
        
        private readonly PactConfig config;

        public MessageBuilderTests()
        {
            this.mockDriver = new Mock<IMessageInteractionDriver>();

            this.config = new PactConfig { DefaultJsonSettings = new JsonSerializerOptions() };

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
        public void WithComment_WhenCalled_AddsComment()
        {
            var expectedKey = "testKey";
            var expectedValue = "testValue";

            this.builder.WithComment(expectedKey, expectedValue);

            this.mockDriver.Verify(s => s.WithComment(expectedKey, expectedValue));
        }

        [Fact]
        public void WithTextComment_WhenCalled_AddsTextComment()
        {
            var expectedComment = "This is a test comment";

            this.builder.WithTextComment(expectedComment);

            this.mockDriver.Verify(s => s.WithTextComment(expectedComment));
        }

        [Fact]
        public void WithAsyncApiReference_WhenCalled_AddsAsyncApiReference()
        {
            var expectedOperationId = "sendEmailMessage";

            this.builder.WithAsyncApiReference(expectedOperationId);

            this.mockDriver.Verify(s => s.WithComment(
                "references",
                It.Is<string>(json =>
                    json.Contains("AsyncAPI") &&
                    json.Contains("operationId") &&
                    json.Contains(expectedOperationId)
                )
            ));
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

            this.builder.WithJsonContent(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            this.mockDriver.Verify(s => s.WithContents("application/json", expected, 0));
        }

        [Fact]
        public void WithJsonContent_MatcherProperties_AddsContent()
        {
            dynamic content = new { Matcher = Match.Integer(42) };
            const string expected = @"{""matcher"":{""pact:matcher:type"":""integer"",""value"":42}}";

            this.builder.WithJsonContent(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            this.mockDriver.Verify(s => s.WithContents("application/json", expected, 0));
        }
    }
}
