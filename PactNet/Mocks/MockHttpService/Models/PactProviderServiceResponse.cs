using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class PactProviderServiceResponse
    {
        [JsonProperty(PropertyName = "status")]
        public HttpStatusCode Status { get; set; }

        [JsonProperty(PropertyName = "headers")]
        public Dictionary<string, string> Headers { get; set; }

        [JsonProperty(PropertyName = "body")]
        public dynamic Body { get; set; }
    }
}