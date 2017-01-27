using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PactNet.Extensions;

namespace PactNet.Models.Messaging
{
    public class PactMessageFile : PactDetails
    {
        private List<Message> messages;

        public PactMessageFile()
        {
            messages = new List<Message>();
            MetaData = new Dictionary<string, object>();

            MetaData["pact-specification"] = "3.0.0";
            MetaData["pact-net"] = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        [JsonProperty(PropertyName = "messages", Order = -1)]
        public Message [] Messages
        {
            get { return messages.ToArray(); } 
            set { messages = value.ToList(); }
        }

        [JsonProperty(PropertyName = "metadata", NullValueHandling = NullValueHandling.Ignore, Order=0)]
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

        public override string GeneratePactFileName()
        {
            return String.Format("{0}-{1}.json",
                Consumer != null ? Consumer.Name : String.Empty,
                Provider != null ? Provider.Name : String.Empty)
                .ToSnakeCase();
        }
    }
}
