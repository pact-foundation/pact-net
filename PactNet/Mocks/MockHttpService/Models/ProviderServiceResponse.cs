using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServiceResponse : IHttpMessage
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "headers")]
        public IDictionary<string, string> Headers { get; set; }

        [JsonProperty(PropertyName = "body")]
        public dynamic Body { get; set; }
    }
}