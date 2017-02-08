using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Configuration.Json;
using PactNet.Models.ProviderService;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
    public class ProviderServiceRequestTests
    {
        [Fact]
        public void PathWithQuery_WithNullPathAndQuery_ReturnsNull()
        {
            var request = new ProviderServiceRequest();

            var uri = request.PathWithQuery();

            Assert.Null(uri);
        }

        [Fact]
        public void PathWithQuery_WithJustPath_ReturnsPath()
        {
            var request = new ProviderServiceRequest
            {
                Path = "/events"
            };

            var uri = request.PathWithQuery();

            Assert.Equal(request.Path, uri);
        }

        [Fact]
        public void PathWithQuery_WithJustQuery_ThrowsInvalidOperationException()
        {
            var request = new ProviderServiceRequest
            {
                Query = "test1=1&test2=2"
            };

            Assert.Throws<InvalidOperationException>(() => request.PathWithQuery());
        }

        [Fact]
        public void PathWithQuery_WithPathAndQuery_ReturnsPathWithQuery()
        {
            var request = new ProviderServiceRequest
            {
                Path = "/events",
                Query = "test1=1&test2=2"
            };

            var uri = request.PathWithQuery();

            Assert.Equal(request.Path + "?" + request.Query, uri);
        }

        [Fact]
        public void SerializeObject_WithDefaultApiSerializerSettings_ReturnsCorrectJson()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = new
                {
                    Test1 = "hi",
                    test2 = 2
                }
            };

            var requestJson = JsonConvert.SerializeObject(request, JsonConfig.ApiSerializerSettings);
            var expectedJson = "{\"method\":\"get\",\"headers\":{\"Content-Type\":\"application/json\"},\"body\":{\"Test1\":\"hi\",\"test2\":2}}";
            Assert.Equal(expectedJson, requestJson);
        }

        [Fact]
        public void SerializeObject_WithDefaultApiSerializerSettingsAndNoHeadersOrBody_ReturnsCorrectJson()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Get
            };

            var requestJson = JsonConvert.SerializeObject(request, JsonConfig.ApiSerializerSettings);
            var expectedJson = "{\"method\":\"get\"}";
            Assert.Equal(expectedJson, requestJson);
        }

        [Fact]
        public void SerializeObject_WithCamelCaseApiSerializerSettings_ReturnsCorrectJson()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                Body = new
                {
                    Test1 = "hi",
                    test2 = 2
                }
            };

            var requestJson = JsonConvert.SerializeObject(request, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            var expectedJson = "{\"method\":\"get\",\"headers\":{\"Content-Type\":\"application/json\"},\"body\":{\"test1\":\"hi\",\"test2\":2}}";
            Assert.Equal(expectedJson, requestJson);
        }
    }
}
