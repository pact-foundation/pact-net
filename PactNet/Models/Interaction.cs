using Newtonsoft.Json;

namespace PactNet.Models
{
    public class Interaction
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        [JsonProperty(Order = -3, PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(Order = -2, PropertyName = "provider_state")]
        public string ProviderState { get; set; }

        public string AsJsonString()
        {
            return JsonConvert.SerializeObject(this, _jsonSerializerSettings);
        }
    }
}