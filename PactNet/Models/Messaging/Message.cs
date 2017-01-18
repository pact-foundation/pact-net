using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Matchers;
using PactNet.Models.Messaging.Consumer.Dsl;

namespace PactNet.Models.Messaging
{
    public class Message
    {
        public Message()
        {
        }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "providerState")]
        public string ProviderState { get; set; }

        [JsonIgnore]
        public PactDslJsonBody Body { get; set; }

        [JsonProperty("contents", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Contents => Body?.Content;

        [JsonProperty("matchingRules", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, List<IMatcher>> MatchingRules => Body?.Matchers;

      
    }
}
