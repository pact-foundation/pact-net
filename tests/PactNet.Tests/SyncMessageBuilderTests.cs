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
    public class SyncMessageBuilderTests
    {
        private readonly ISyncMessageBuilderV4 builder;

        private readonly Mock<ISyncMessageInteractionDriver> mockDriver;

        private readonly PactConfig config;

        public SyncMessageBuilderTests()
        {
            this.mockDriver = new Mock<ISyncMessageInteractionDriver>();

            this.config = new PactConfig { DefaultJsonSettings = new JsonSerializerOptions() };

            this.builder = new SyncMessageBuilder(this.mockDriver.Object, this.config, PactSpecification.V4);
        }

        [Fact]
        public void Ctor_Throws_Exception_If_Driver_Not_Set()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SyncMessageBuilder(null, new PactConfig(), PactSpecification.V4));
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
        public void WithRequestMetadata_WhenCalled_AddsRequestMetadata()
        {
            this.builder.WithRequestMetadata("poolId", "1234");

            this.mockDriver.Verify(s => s.WithRequestMetadata("poolId", "1234"));
        }

        [Fact]
        public void WithResponseMetadata_WhenCalled_AddsResponseMetadata()
        {
            this.builder.WithResponseMetadata("poolId", "1234");

            this.mockDriver.Verify(s => s.WithResponseMetadata("poolId", "1234"));
        }

        [Fact]
        public void WithRequestJsonContent_WithoutCustomSettings_AddsContentWithDefaultSettings()
        {
            var content = new { Id = 1, Desc = "description" };
            const string expected = @"{""Id"":1,""Desc"":""description""}";

            this.builder.WithRequestJsonContent(content);

            this.mockDriver.Verify(s => s.WithRequestContents("application/json", expected));
        }

        [Fact]
        public void WithRequestJsonContent_WithCustomSettings_AddsContentWithOverriddenSettings()
        {
            var content = new { Id = 1, Desc = "description" };
            const string expected = @"{""id"":1,""desc"":""description""}";

            this.builder.WithRequestJsonContent(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            this.mockDriver.Verify(s => s.WithRequestContents("application/json", expected));
        }

        [Fact]
        public void WithRequestJsonContent_MatcherProperties_AddsContent()
        {
            dynamic content = new { Matcher = Match.Integer(42) };
            const string expected = @"{""matcher"":{""pact:matcher:type"":""integer"",""value"":42}}";

            this.builder.WithRequestJsonContent(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            this.mockDriver.Verify(s => s.WithRequestContents("application/json", expected));
        }

        [Fact]
        public void WithResponseJsonContent_WithoutCustomSettings_AddsContentWithDefaultSettings()
        {
            var content = new { Id = 1, Desc = "description" };
            const string expected = @"{""Id"":1,""Desc"":""description""}";

            this.builder.WithResponseJsonContent(content);

            this.mockDriver.Verify(s => s.WithResponseContents("application/json", expected));
        }

        [Fact]
        public void WithResponseJsonContent_ReturnsConfiguredVerifier()
        {
            var content = new { Id = 1 };

            IConfiguredSyncMessageVerifier verifier = this.builder.WithResponseJsonContent(content);

            Assert.NotNull(verifier);
        }
    }
}
