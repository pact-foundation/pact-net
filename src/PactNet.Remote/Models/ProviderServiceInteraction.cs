using Newtonsoft.Json;

namespace PactNet.Remote.Models
{
    public class ProviderServiceInteraction : Interaction
    {
        [JsonProperty(PropertyName = "request", NullValueHandling = NullValueHandling.Ignore)]
        public ProviderServiceRequest Request { get; set; }

        [JsonProperty(PropertyName = "response", NullValueHandling = NullValueHandling.Ignore)]
        public ProviderServiceResponse Response { get; set; }
    }
}