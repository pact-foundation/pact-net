using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Configuration.Json;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Models
{
    public class ProviderServiceRequestTests
    {
        [Fact]
        public void SerializeObject_WithDefaultApiSerializerSettings_ReturnsCorrectJson()
        {
            var request = new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Headers = new Dictionary<string, object> { { "Content-Type", "application/json" } },
                Body = new
                {
                    Test1 = "hi",
                    test2 = 2,
                    test3 = (string)null
                }
            };

            var requestJson = JsonConvert.SerializeObject(request, JsonConfig.ApiSerializerSettings);
            var expectedJson = "{\"method\":\"get\",\"headers\":{\"Content-Type\":\"application/json\"},\"body\":{\"Test1\":\"hi\",\"test2\":2,\"test3\":null}}";
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
                Headers = new Dictionary<string, object> { { "Content-Type", "application/json" } },
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
