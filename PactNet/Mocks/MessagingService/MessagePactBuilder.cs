using Newtonsoft.Json;
using PactNet.Mocks;
using PactNet.Models.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PactNet.Mocks.MockHttpService;
using PactNet.Extensions;
using System.IO;

namespace PactNet.Mocks.MessagingService
{
    public class MessagePactBuilder : IPactMessagingBuilder
    {
        private readonly PactMessageFile pactMessage;
        private string expectedMessageTopic;
        private readonly PactConfig pactConfig;

        public MessagePactBuilder() 
            : this(new PactConfig())
        {
           
        }

        public MessagePactBuilder(PactConfig pactConfig)
        {
            this.pactMessage = new PactMessageFile();
            this.expectedMessageTopic = string.Empty;
            this.pactConfig = pactConfig;
        }

        public void AddMessage(Message message)
        {
            this.pactMessage.AddMessage(message);
        }

        public void ExceptsToRecieve(string messageTopic)
        {
            expectedMessageTopic = messageTopic;
        }

        public Message GetMessage()
        {
            return this.pactMessage.GetMessage();
        }


        public string GetPactAsJSON()
        {
            return JsonConvert.SerializeObject(pactMessage);
        }

        public IPactBuilder ServiceConsumer(string consumerName)
        {
            if (String.IsNullOrWhiteSpace(consumerName))
            {
                throw new ArgumentException("Please supply a non null or empty consumerName");
            }

            pactMessage.Consumer = new Models.Pacticipant() { Name = consumerName };

            return this;
        }

        public IPactBuilder HasPactWith(string providerName)
        {
            if (String.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Please supply a non null or empty providerName");
            }

            pactMessage.Provider = new Models.Pacticipant() { Name = providerName };

            return this;
        }

        public void Build()
        {
            if (String.IsNullOrWhiteSpace(pactMessage.Consumer.Name))
            {
                throw new InvalidOperationException("ConsumerName has not been set, please supply a consumer name using the ServiceConsumer method.");
            }

            if (String.IsNullOrWhiteSpace(pactMessage.Provider.Name))
            {
                throw new InvalidOperationException("ProviderName has not been set, please supply a provider name using the HasPactWith method.");
            }


            PersistPactFileToDisk();
        }

        private string GeneratePactFileName()
        {
            return String.Format("{0}-{1}-{2}.json",
                pactMessage.Consumer != null ? pactMessage.Consumer.Name.Replace('.', '-') : String.Empty,
                pactMessage.Provider != null ? pactMessage.Provider.Name.Replace('.', '-') : String.Empty,
                expectedMessageTopic != null ? expectedMessageTopic.Replace('.', '-') : String.Empty)
                .ToLowerSnakeCase();
        }

        private void PersistPactFileToDisk()
        {
            string fileName = GeneratePactFileName();

            if(!Directory.Exists(this.pactConfig.PactDir))
            {
                Directory.CreateDirectory(this.pactConfig.PactDir);
            }

            string fullPath = this.pactConfig.PactDir + fileName;

            File.WriteAllText(fullPath, this.GetPactAsJSON());
        }
    }
}
