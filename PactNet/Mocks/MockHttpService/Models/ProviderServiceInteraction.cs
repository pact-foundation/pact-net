using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class ProviderServiceInteraction : Interaction
    {
        [JsonProperty(PropertyName = "request")]
        public ProviderServiceRequest Request { get; set; }

        [JsonProperty(PropertyName = "response")]
        public ProviderServiceResponse Response { get; set; }
    }
}