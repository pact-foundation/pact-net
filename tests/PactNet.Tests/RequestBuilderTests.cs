using System;
using System.Collections.Generic;
using System.Net.Http;
using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Generators;
using PactNet.Interop;
using Xunit;
using Match = PactNet.Matchers.Match;

namespace PactNet.Tests
{
    public class RequestBuilderTests
    {
        private readonly RequestBuilder builder;

        private readonly Mock<IMockServer> mockServer;

        private readonly IFixture fixture;
        private readonly InteractionHandle handle;
        private readonly JsonSerializerSettings settings;

        public RequestBuilderTests()
        {
            this.mockServer = new Mock<IMockServer>();

            this.fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(this.fixture);

            this.handle = this.fixture.Create<InteractionHandle>();
            this.settings = new JsonSerializerSettings();

            this.builder = new RequestBuilder(this.mockServer.Object, this.handle, this.settings);
        }

        [Fact]
        public void Given_WhenCalled_AddsProviderState()
        {
            this.builder.Given("provider state");

            this.mockServer.Verify(s => s.Given(this.handle, "provider state"));
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

            this.mockServer.Verify(s => s.GivenWithParam(this.handle, "provider state", "foo", "bar"));
            this.mockServer.Verify(s => s.GivenWithParam(this.handle, "provider state", "baz", "bash"));
        }

        [Fact]
        public void WithRequest_HttpMethod_AddsRequest()
        {
            this.builder.WithRequest(HttpMethod.Post, "/some/path");

            this.mockServer.Verify(s => s.WithRequest(this.handle, "POST", "/some/path"));
        }

        [Fact]
        public void WithRequest_HttpMethod_ProviderStateGenerator_AddsRequest()
        {
            const string example = "/some/example-path";
            const string expression = "/some/${path}";
            const string expectedValue = $@"{{""pact:matcher:type"":""type"",""value"":""{example}"",""pact:generator:type"":""ProviderState"",""expression"":""{expression}""}}";

            this.builder.WithRequest(HttpMethod.Post, Generate.ProviderState(example, expression));

            this.mockServer.Verify(s => s.WithRequest(this.handle, "POST", expectedValue));
        }

        [Fact]
        public void WithRequest_String_AddsRequest()
        {
            this.builder.WithRequest("POST", "/some/path");

            this.mockServer.Verify(s => s.WithRequest(this.handle, "POST", "/some/path"));
        }

        [Fact]
        public void WithRequest_String_ProviderStateGenerator_AddsRequest()
        {
            const string example = "/some/example-path";
            const string expression = "/some/${path}";
            const string expectedValue = $@"{{""pact:matcher:type"":""type"",""value"":""{example}"",""pact:generator:type"":""ProviderState"",""expression"":""{expression}""}}";

            this.builder.WithRequest("POST", Generate.ProviderState(example, expression));

            this.mockServer.Verify(s => s.WithRequest(this.handle, "POST", expectedValue));
        }

        [Fact]
        public void WithQuery_WhenCalled_AddsQueryParam()
        {
            this.builder.WithQuery("name", "value");

            this.mockServer.Verify(s => s.WithQueryParameter(this.handle, "name", "value", 0));
        }

        [Fact]
        public void WithQuery_Generator_WhenCalled_AddsQueryParam()
        {
            const string expectedValue = $@"{{""pact:matcher:type"":""type"",""value"":""example"",""pact:generator:type"":""ProviderState"",""expression"":""${{value}}""}}";

            this.builder.WithQuery("name", Generate.ProviderState("example", "${value}"));
            this.mockServer.Verify(s => s.WithQueryParameter(this.handle, "name", expectedValue, 0));
        }

        [Fact]
        public void WithQuery_RepeatedQuery_SetsIndex()
        {
            this.builder.WithQuery("name", "value1");
            this.builder.WithQuery("name", "value2");
            this.builder.WithQuery("other", "value");

            this.mockServer.Verify(s => s.WithQueryParameter(this.handle, "name", "value1", 0));
            this.mockServer.Verify(s => s.WithQueryParameter(this.handle, "name", "value2", 1));
            this.mockServer.Verify(s => s.WithQueryParameter(this.handle, "other", "value", 0));
        }

        [Fact]
        public void WithQuery_Generator_RepeatedQuery_SetsIndex()
        {
            const string expectedValue2 = $@"{{""pact:matcher:type"":""type"",""value"":""value2"",""pact:generator:type"":""ProviderState"",""expression"":""${{value}}""}}";

            this.builder.WithQuery("name", "value1");
            this.builder.WithQuery("name", Generate.ProviderState("value2", "${value}"));
            this.builder.WithQuery("other", "value");

            this.mockServer.Verify(s => s.WithQueryParameter(this.handle, "name", "value1", 0));
            this.mockServer.Verify(s => s.WithQueryParameter(this.handle, "name", expectedValue2, 1));
            this.mockServer.Verify(s => s.WithQueryParameter(this.handle, "other", "value", 0));
        }

        [Fact]
        public void WithHeader_Matcher_WhenCalled_AddsSerialisedHeaderParam()
        {
            var expectedValue = "{\"pact:matcher:type\":\"regex\",\"value\":\"header\",\"regex\":\"^header$\"}";

            this.builder.WithHeader("name", Match.Regex("header", "^header$"));

            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "name", expectedValue, 0));
        }

        [Fact]
        public void WithHeader_Generator_WhenCalled_AddsSerialisedHeaderParam()
        {
            var expectedValue = "{\"pact:matcher:type\":\"type\",\"value\":\"header\",\"pact:generator:type\":\"ProviderState\",\"expression\":\"${header}\"}";

            this.builder.WithHeader("name", Generate.ProviderState("header", "${header}"));

            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "name", expectedValue, 0));
        }

        [Fact]
        public void WithHeader_RepeatedMatcherHeader_SetsIndex()
        {
            var expectedValue1 = "{\"pact:matcher:type\":\"regex\",\"value\":\"value1\",\"regex\":\"^value1$\"}";
            var expectedValue2 = "{\"pact:matcher:type\":\"type\",\"value\":\"value2\"}";
            var expectedValue = "{\"pact:matcher:type\":\"regex\",\"value\":\"value\",\"regex\":\"^value$\"}";

            this.builder.WithHeader("name", Match.Regex("value1", "^value1$"));
            this.builder.WithHeader("name", Match.Type("value2"));
            this.builder.WithHeader("other", Match.Regex("value", "^value$"));

            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "name", expectedValue1, 0));
            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "name", expectedValue2, 1));
            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "other", expectedValue, 0));
        }

        [Fact]
        public void WithHeader_String_WhenCalled_AddsHeaderParam()
        {
            this.builder.WithHeader("name", "value");

            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "name", "value", 0));
        }

        [Fact]
        public void WithHeader_RepeatedStringHeader_SetsIndex()
        {
            this.builder.WithHeader("name", "value1");
            this.builder.WithHeader("name", "value2");
            this.builder.WithHeader("other", "value");

            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "name", "value1", 0));
            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "name", "value2", 1));
            this.mockServer.Verify(s => s.WithRequestHeader(this.handle, "other", "value", 0));
        }

        [Fact]
        public void WithJsonBody_WithoutCustomSettings_AddsRequestBodyWithDefaultSettings()
        {
            this.builder.WithJsonBody(new { Foo = 42 });

            this.mockServer.Verify(s => s.WithRequestBody(this.handle, "application/json", @"{""Foo"":42}"));
        }

        [Fact]
        public void WithJsonBody_WithCustomSettings_AddsRequestBodyWithOverriddenSettings()
        {
            this.builder.WithJsonBody(new { Foo = 42 },
                                      new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            this.mockServer.Verify(s => s.WithRequestBody(this.handle, "application/json", @"{""foo"":42}"));
        }

        [Fact]
        public void WillRespond_RequestConfigured_ReturnsResponseBuilder()
        {
            this.builder.WithRequest(HttpMethod.Delete, "/foo");

            var responseBuilder = this.builder.WillRespond();

            responseBuilder.Should().BeOfType<ResponseBuilder>();
        }

        [Fact]
        public void WillRespond_RequestNotConfigured_ThrowsInvalidOperationException()
        {
            Action action = () => this.builder.WillRespond();

            action.Should().Throw<InvalidOperationException>("because the request has not been configured");
        }
    }
}
