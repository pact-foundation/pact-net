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
using System.Net.Http;
using System.Net.Http.Headers;

namespace PactNet
{
    public class PactMessageBuilder : IPactMessagingBuilder
    {
        private readonly MessagingPactFile pactMessage;
        private readonly PactConfig pactConfig;

        public PactMessageBuilder() 
            : this(new PactConfig())
        {
   
        }

        public PactMessageBuilder(PactConfig pactConfig)
        {
            this.pactMessage = new MessagingPactFile();
            this.pactConfig = pactConfig;
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



        public IPactMessagingBuilder WithContent(Message message)
        {
            this.pactMessage.AddMessage(message);
            return this;
        }

        private void PersistPactFileToDisk()
        {
            string fileName = this.pactMessage.GeneratePactFileName();

            if(!Directory.Exists(this.pactConfig.PactDir))
            {
                Directory.CreateDirectory(this.pactConfig.PactDir);
            }

            string fullPath = this.pactConfig.PactDir + fileName;

            File.WriteAllText(fullPath, this.GetPactAsJSON());
        }

       
    }
}
