using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Configuration.Json.Converters;
using PactNet.Matchers;
using PactNet.Models.Consumer.Dsl;

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

        [JsonProperty("metaData", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> MetaData { get; set; }


        #region Builder
        public Message Given(string providerState)
        {
            this.ProviderState = providerState;
            return this;
        }

        public Message ExpectsToRecieve(string description)
        {
            this.Description = description;
            return this;
        }

        public Message WithMetaData(Dictionary<string, object> metadata)
        {
            this.MetaData = metadata;
            return this;
        }

        public Message WithBody(PactDslJsonBody body)
        {
            this.Body = body;
            return this;
        }

        public Message WithBody(dynamic body)
        {
            this.Body = PactDslJsonBody.Parse(body);
            return this;
        }
        #endregion
    }
}
