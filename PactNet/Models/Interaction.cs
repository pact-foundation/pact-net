using System;
using Newtonsoft.Json;

namespace PactNet.Models
{
    public class Interaction
    {
        [JsonProperty(Order = -3, PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(Order = -2, PropertyName = "provider_state")]
        public string ProviderState { get; set; }

        public string Summary()
        {
            if (!String.IsNullOrEmpty(Description) && !String.IsNullOrEmpty(ProviderState))
            {
                return String.Format("{0} - {1}", Description, ProviderState);
            }

            return Description;
        }
    }
}