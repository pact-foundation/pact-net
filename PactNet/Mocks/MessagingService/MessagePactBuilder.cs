using Newtonsoft.Json;
using PactNet.Mocks;
using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Mocks.MessagingService
{
    public class MessagePactBuilder<T> : IMockMessager<T>
    {
        private PactMessageFile<T> pactMessage;

        public MessagePactBuilder()
        {
            pactMessage = new PactMessageFile<T>();
        }

        public void AddMessage(Message<T> message)
        {
            this.pactMessage.AddMessage(message);
        }

        public void ExceptsToRecieve(string messageTopic)
        {
            throw new NotImplementedException();
        }

        public Message<T> GetMessage()
        {
            return this.pactMessage.GetMessage();
        }

        public void GivenConsumer(string consumer)
        {
            pactMessage.Consumer = new Models.Pacticipant() { Name = consumer };
        }

        public void GivenProvider(string provider)
        {
            pactMessage.Provider = new Models.Pacticipant() { Name = provider };
        }

        public string Build()
        {
            return JsonConvert.SerializeObject(pactMessage);
        }
    }
}
