using System;
using System.Collections.Generic;
using System.Net.Http;
using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Drivers;
using Xunit;
using Match = PactNet.Matchers.Match;

namespace PactNet.Tests
{
    public class RequestBuilderTests
    {
        private readonly RequestBuilder builder;

        private readonly Mock<IHttpInteractionDriver> mockDriver;

        private readonly IFixture fixture;
        private readonly JsonSerializerSettings settings;

        public RequestBuilderTests()
        {
            this.mockDriver = new Mock<IHttpInteractionDriver>();
            
            this.settings = new JsonSerializerSettings();

            this.builder = new RequestBuilder(this.mockDriver.Object, this.settings);
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
        public void WithRequest_HttpMethod_AddsRequest()
        {
            this.builder.WithRequest(HttpMethod.Post, "/some/path");

            this.mockDriver.Verify(s => s.WithRequest("POST", "/some/path"));
        }

        [Fact]
        public void WithRequest_String_AddsRequest()
        {
            this.builder.WithRequest("POST", "/some/path");

            this.mockDriver.Verify(s => s.WithRequest("POST", "/some/path"));
        }

        [Fact]
        public void WithQuery_WhenCalled_AddsQueryParam()
        {
            this.builder.WithQuery("name", "value");

            this.mockDriver.Verify(s => s.WithQueryParameter("name", "value", 0));
        }

        [Fact]
        public void WithQuery_RepeatedQuery_SetsIndex()
        {
            this.builder.WithQuery("name", "value1");
            this.builder.WithQuery("name", "value2");
            this.builder.WithQuery("other", "value");

            this.mockDriver.Verify(s => s.WithQueryParameter("name", "value1", 0));
            this.mockDriver.Verify(s => s.WithQueryParameter("name", "value2", 1));
            this.mockDriver.Verify(s => s.WithQueryParameter("other", "value", 0));
        }

        [Fact]
        public void WithHeader_Matcher_WhenCalled_AddsSerialisedHeaderParam()
        {
            var expectedValue = "{\"pact:matcher:type\":\"regex\",\"value\":\"header\",\"regex\":\"^header$\"}";

            this.builder.WithHeader("name", Match.Regex("header", "^header$"));

            this.mockDriver.Verify(s => s.WithRequestHeader("name", expectedValue, 0));
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

            this.mockDriver.Verify(s => s.WithRequestHeader("name", expectedValue1, 0));
            this.mockDriver.Verify(s => s.WithRequestHeader("name", expectedValue2, 1));
            this.mockDriver.Verify(s => s.WithRequestHeader("other", expectedValue, 0));
        }

        [Fact]
        public void WithHeader_String_WhenCalled_AddsHeaderParam()
        {
            this.builder.WithHeader("name", "value");

            this.mockDriver.Verify(s => s.WithRequestHeader("name", "value", 0));
        }

        [Fact]
        public void WithHeader_RepeatedStringHeader_SetsIndex()
        {
            this.builder.WithHeader("name", "value1");
            this.builder.WithHeader("name", "value2");
            this.builder.WithHeader("other", "value");

            this.mockDriver.Verify(s => s.WithRequestHeader("name", "value1", 0));
            this.mockDriver.Verify(s => s.WithRequestHeader("name", "value2", 1));
            this.mockDriver.Verify(s => s.WithRequestHeader("other", "value", 0));
        }

        [Fact]
        public void WithJsonBody_NoOverrides_AddsRequestBodyWithDefaultSettings()
        {
            this.builder.WithJsonBody(new { Foo = 42 });

            this.mockDriver.Verify(s => s.WithRequestBody("application/json", @"{""Foo"":42}"));
        }

        [Fact]
        public void WithJsonBody_OverrideContentType_AddsRequestBodyWithOverriddenContentType()
        {
            this.builder.WithJsonBody(new { Foo = 42 }, "application/json-patch+json");

            this.mockDriver.Verify(s => s.WithRequestBody("application/json-patch+json", @"{""Foo"":42}"));
        }

        [Fact]
        public void WithJsonBody_OverrideJsonSettings_AddsRequestBodyWithOverriddenSettings()
        {
            this.builder.WithJsonBody(new { Foo = 42 },
                                      new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            this.mockDriver.Verify(s => s.WithRequestBody("application/json", @"{""foo"":42}"));
        }

        [Fact]
        public void WithJsonBody_OverrideContentTypeAndSettings_AddsRequestBodyWithOverriddenContentTypeAndSettings()
        {
            this.builder.WithJsonBody(new { Foo = 42 },
                                      new JsonSerializerSettings
                                      {
                                          ContractResolver = new CamelCasePropertyNamesContractResolver()
                                      },
                                      "application/json-patch+json");

            this.mockDriver.Verify(s => s.WithRequestBody("application/json-patch+json", @"{""foo"":42}"));
        }

        [Fact]
        public void WithBody_WhenCalled_AddsRequestBody()
        {
            this.builder.WithBody("foo,bar\nbaz,bash", "text/csv");

            this.mockDriver.Verify(s => s.WithRequestBody("text/csv", "foo,bar\nbaz,bash"));
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
