using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Matchers;
using PactNet.Models.Messaging.Consumer.Dsl;

namespace PactNet.Models.Messaging
{
    [JsonConverter(typeof(MessageJsonConverter))]
    public class Message
    {
        public Message()
        {
            this.Body = new PactDslJsonBody();
        }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "providerState")]
        public string ProviderState { get; set; }

        [JsonIgnore]
        public PactDslJsonBody Body { get; set; }

        [JsonProperty("contents", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> Contents
        {
            get { return this.Body.Content; }
            set { this.Body.Content = value; }
        }

        [JsonProperty("matchingRules", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> MatchingRules
        {
            get { return this.Body.Matchers; }
            set { this.Body.Matchers = value; }
        }
    }
}
