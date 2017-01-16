using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Models.Messaging
{
    public class PactMessageFile<T> : PactDetails
    {
        private List<Message<T>> messages;

        public PactMessageFile()
        {
            messages = new List<Message<T>>();
        }

        [JsonProperty(PropertyName = "messages")]
        public Message<T> [] Messages
        {
            get { return messages.ToArray(); } 
        }

        public void AddMessage(Message<T> newMessage)
        {
            messages.Add(newMessage);
        }

        public Message<T> GetMessage()
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
