using System.Collections.Generic;
using Newtonsoft.Json;
using PactNet.Models;

namespace PactNet.PactMessage.Models
{
    public class MessageInteraction : Interaction
    {
        [JsonProperty(Order = 0, PropertyName = "providerStates")]
        public IEnumerable<ProviderState> ProviderStates
        {
            get; set;
        }

        [JsonProperty(Order = 1, PropertyName = "contents")]
        public dynamic Contents
        {
            get; set;
        }

        [JsonProperty(Order = 2, PropertyName = "metadata")]
        public dynamic Metadata
        {
            get; set;
        }
    }
}
