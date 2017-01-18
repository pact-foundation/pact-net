using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Models.Messaging
{
    public class PactMessageFile : PactDetails
    {
        private List<Message> messages;

        public PactMessageFile()
        {
            messages = new List<Message>();
        }

        [JsonProperty(PropertyName = "messages")]
        public Message [] Messages
        {
            get { return messages.ToArray(); } 
        }

        [JsonProperty(PropertyName = "metaData", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, object> MetaData { get; set; }

        public void AddMessage(Message newMessage)
        {
            messages.Add(newMessage);
        }

        public Message GetMessage()
        {
            var m = messages.FirstOrDefault();

            if(messages.Count > 0)
            {
                messages.RemoveAt(0);
            }

            return m;
        }

        
    }
}
