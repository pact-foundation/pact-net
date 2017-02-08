﻿using Newtonsoft.Json;
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
        {
            messages = new List<Message>();


        }

        [JsonProperty(PropertyName = "messages", Order = -1)]
        public Message [] Messages
        {
            get { return messages.ToArray(); } 
            set { messages = value.ToList(); }
        }

        //[JsonProperty(PropertyName = "metadata", NullValueHandling = NullValueHandling.Ignore, Order=0)]
        //public Dictionary<string, object> MetaData { get; set; }

        public void AddMessage(Message newMessage)
        {
            messages.Add(newMessage);
        }

    }
}
