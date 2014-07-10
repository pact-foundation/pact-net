using Newtonsoft.Json;

namespace PactNet.Models
{
    public class PactInteraction
    {
        [JsonProperty(Order = -3)]
        public string Description { get; set; }

        [JsonProperty(Order = -2)]
        public string ProviderState { get; set; }
    }
}