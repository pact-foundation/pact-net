using Newtonsoft.Json;

namespace PactNet.Models.ProviderService
{
    public class ProviderServiceInteraction : Interaction
    {
        [JsonProperty(PropertyName = "request")]
        public ProviderServiceRequest Request { get; set; }

        [JsonProperty(PropertyName = "response")]
        public ProviderServiceResponse Response { get; set; }
    }
}