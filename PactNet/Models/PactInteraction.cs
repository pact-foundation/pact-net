using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactInteraction
    {
        [JsonProperty(Order = -3, PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(Order = -2, PropertyName = "providerState")]
        public string ProviderState { get; set; }
    }
}