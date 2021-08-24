using System.Net;
using AutoFixture;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Native.Interop;
using Xunit;

namespace PactNet.Native.Tests
{
    public class NativeResponseBuilderTests
    {
        private readonly NativeResponseBuilder builder;

        private readonly Mock<IMockServer> mockServer;

        private readonly IFixture fixture;
        private readonly InteractionHandle handle;
        private readonly JsonSerializerSettings settings;

        public NativeResponseBuilderTests()
        {
            this.mockServer = new Mock<IMockServer>();

            this.fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(this.fixture);

            this.handle = this.fixture.Create<InteractionHandle>();
            this.settings = new JsonSerializerSettings();

            this.builder = new NativeResponseBuilder(this.mockServer.Object, this.handle, this.settings);
        }

        [Fact]
        public void WithStatus_HttpStatusCode_SetsStatus()
        {
            this.builder.WithStatus(HttpStatusCode.Unauthorized);

            this.mockServer.Verify(s => s.ResponseStatus(this.handle, 401));
        }

        [Fact]
        public void WithStatus_Int_SetsStatus()
        {
            this.builder.WithStatus(429);

            this.mockServer.Verify(s => s.ResponseStatus(this.handle, 429));
        }

        [Fact]
        public void WithHeader_Matcher_WhenCalled_AddsSerialisedHeaderParam()
        {
            this.builder.WithHeader("name", PactNet.Matchers.Match.Regex("header", "^header$"));

            var expectedValue = "{\"pact:matcher:type\":\"regex\",\"value\":\"header\",\"regex\":\"^header$\"}";

            this.mockServer.Verify(s => s.WithResponseHeader(this.handle, "name", expectedValue, 0));
        }

        [Fact]
        public void WithHeader_Matcher_RepeatedHeader_SetsIndex()
        {
            this.builder.WithHeader("name", PactNet.Matchers.Match.Regex("value1", "^value1$"));
            this.builder.WithHeader("name", PactNet.Matchers.Match.Type("value2"));
            this.builder.WithHeader("other", PactNet.Matchers.Match.Regex("value", "^value$"));

            var expectedValue1 = "{\"pact:matcher:type\":\"regex\",\"value\":\"value1\",\"regex\":\"^value1$\"}";
            var expectedValue2 = "{\"pact:matcher:type\":\"type\",\"value\":\"value2\"}";
            var expectedValue = "{\"pact:matcher:type\":\"regex\",\"value\":\"value\",\"regex\":\"^value$\"}";

            this.mockServer.Verify(s => s.WithResponseHeader(this.handle, "name", expectedValue1, 0));
            this.mockServer.Verify(s => s.WithResponseHeader(this.handle, "name", expectedValue2, 1));
            this.mockServer.Verify(s => s.WithResponseHeader(this.handle, "other", expectedValue, 0));
        }

        [Fact]
        public void WithHeader_String_WhenCalled_AddsHeaderParam()
        {
            this.builder.WithHeader("name", "value");

            this.mockServer.Verify(s => s.WithResponseHeader(this.handle, "name", "value", 0));
        }

        [Fact]
        public void WithHeader_String_RepeatedHeader_SetsIndex()
        {
            this.builder.WithHeader("name", "value1");
            this.builder.WithHeader("name", "value2");
            this.builder.WithHeader("other", "value");

            this.mockServer.Verify(s => s.WithResponseHeader(this.handle, "name", "value1", 0));
            this.mockServer.Verify(s => s.WithResponseHeader(this.handle, "name", "value2", 1));
            this.mockServer.Verify(s => s.WithResponseHeader(this.handle, "other", "value", 0));
        }

        [Fact]
        public void WithJsonBody_WithoutCustomSettings_AddsRequestBodyWithDefaultSettings()
        {
            this.builder.WithJsonBody(new { Foo = 42 });

            this.mockServer.Verify(s => s.WithResponseBody(this.handle, "application/json", @"{""Foo"":42}"));
        }

        [Fact]
        public void WithJsonBody_WithCustomSettings_AddsRequestBodyWithOverriddenSettings()
        {
            this.builder.WithJsonBody(new { Foo = 42 },
                                      new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            this.mockServer.Verify(s => s.WithResponseBody(this.handle, "application/json", @"{""foo"":42}"));
        }
    }
}
