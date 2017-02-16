using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PactNet.Extensions;

namespace PactNet.Models.Messaging
{
    public class MessagingPactFile : PactFile
    {
        private List<Message> messages;

        public MessagingPactFile()
            :base("3.0.0")
        {
            messages = new List<Message>();
        }

        [JsonProperty(PropertyName = "messages", Order = -1)]
        public Message [] Messages
        {
            get { return messages.ToArray(); } 
            set { messages = value.ToList(); }
        }

        public void AddMessage(Message newMessage)
        {
            messages.Add(newMessage);
        }
    }
}
