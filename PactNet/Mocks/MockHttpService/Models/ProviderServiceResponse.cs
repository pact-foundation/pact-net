using System.Collections.Generic;
using Newtonsoft.Json;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServiceResponse
    {
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonProperty(PropertyName = "body")]
        public dynamic Body { get; set; }
    }
}