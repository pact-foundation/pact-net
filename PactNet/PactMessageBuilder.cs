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
        private readonly PactMessageFile pactMessage;
        private readonly PactConfig pactConfig;

        public PactMessageBuilder() 
            : this(new PactConfig())
        {
           
        }

        public PactMessageBuilder(PactConfig pactConfig)
        {
            this.pactMessage = new PactMessageFile();
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

        public void PushToBroker(string uri, PactUriOptions brokerSecurityOptions)
        {
            if (String.IsNullOrWhiteSpace(pactMessage.Consumer.Name))
            {
                throw new InvalidOperationException("ConsumerName has not been set, please supply a consumer name using the ServiceConsumer method.");
            }

            if (String.IsNullOrWhiteSpace(pactMessage.Provider.Name))
            {
                throw new InvalidOperationException("ProviderName has not been set, please supply a provider name using the HasPactWith method.");
            }

            doIt(uri);
        }


        public void doIt(string url)
        {
            
            // JsonMediaTypeFormatter jsonMediaTypeFormatter = new JsonMediaTypeFormatter();

            // jsonMediaTypeFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/hal+json"));
            using (HttpClient client = new HttpClient())
            {
                string location = String.Format("/pacts/provider/{0}/consumer/{1}/version/3.0.0", this.pactMessage.Provider.Name, this.pactMessage.Consumer.Name);

                client.BaseAddress = new Uri(url);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/hal+json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes("pact:W0lfpact")));
                // List data response.
                var httpContent = new StringContent(GetPactAsJSON(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PutAsync(location, httpContent).Result;  // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    //// Parse the response body. Blocking!
                    //var rootObject = response.Content.ReadAsAsync(typeof(RootObject), new[] { jsonMediaTypeFormatter }).Result as RootObject;
                    //HttpResponseMessage pactResponse;


                    //client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(
                    //new MediaTypeWithQualityHeaderValue("application/json"));
                    //foreach (var p in rootObject._links.pacts)
                    //{
                    //    var pactUrl = p.href.Replace(URL, "");
                    //    pactResponse = client.GetAsync(pactUrl).Result;
                    //    var pact = pactResponse.Content.ReadAsAsync(typeof(PactRootObject), new[] { jsonMediaTypeFormatter }).Result as PactRootObject;
                    //}
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
            }
        }
        public IPactMessagingBuilder WithContent(Message message)
        {
            this.pactMessage.AddMessage(message);

            return this;
        }

        public IPactMessagingBuilder WithMetaData(Dictionary<string, object> metaData)
        {
            this.pactMessage.MetaData = metaData;
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
