using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Models.Messaging
{
    public class Message<T>
    {
        public Message()
        {
            Contents = new Dictionary<string, T>();
        }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "providerState")]
        public string ProviderState { get; set; }

        [JsonProperty(PropertyName = "contents")]
        public Dictionary<string, T> Contents { get; set; }

        [JsonProperty(PropertyName = "metaData")]
        public MetaData MetaData { get; set; }
    }
}
