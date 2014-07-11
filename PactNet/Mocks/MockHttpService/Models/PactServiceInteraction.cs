using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class PactServiceInteraction : PactInteraction
    {
        [JsonProperty(PropertyName = "request")]
        public PactProviderServiceRequest Request { get; set; }

        [JsonProperty(PropertyName = "response")]
        public PactProviderServiceResponse Response { get; set; }
    }
}