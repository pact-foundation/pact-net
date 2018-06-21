using System;
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

        [JsonProperty(Order = -3, PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(Order = -2, PropertyName = "provider_state", NullValueHandling = NullValueHandling.Ignore)] //provider_state will become providerState
        public string ProviderState { get; set; }

        //[Obsolete("For backwards compatibility.")]
        //public string provider_state { set { ProviderState = value; } } //Uncomment when provider_state becomes providerState
        [Obsolete("For forwards compatibility.")]
        public string providerState { set { ProviderState = value; } } //Remove when provider_state becomes providerState 

        public string AsJsonString()
        {
            return JsonConvert.SerializeObject(this, _jsonSerializerSettings);
        }
    }
}