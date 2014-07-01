using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Concord
{
    public class Pact //TODO: Should we split this into consumer and provider?
    {
        private string _consumerName;
        private string _providerName;
        private PactProvider _pactProvider;
        private const string PactFileDirectory = "C:/specs/pacts/";
        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        public string PactFilePath
        {
            get { return Path.Combine(PactFileDirectory, PactFileName); }
        }

        public string PactFileName 
        {
            get { return String.Format("{0}-{1}.json", _consumerName, _providerName); }
        }

        public Pact()
        {
        }

        public Pact ServiceConsumer(string consumerName)
        {
            _consumerName = consumerName;

            return this;
        }

        public Pact HasPactWith(string providerName)
        {
            _providerName = providerName;

            return this;
        }

        public Pact ServiceProvider(string providerName)
        {
            _providerName = providerName;

            return this;
        }

        public Pact HonoursPactWith(string consumerName, HttpClient client)
        {
            _consumerName = consumerName;

            var pactFileJson = File.ReadAllText(PactFilePath);
            var pactFile = JsonConvert.DeserializeObject<PactFile>(pactFileJson, _jsonSettings);

            pactFile.VerifyProvider(client);

            return this;
        }

        public Pact MockService(int port)
        {
            _pactProvider = new PactProvider(port);

            return this;
        }
         
        /*public Pact PactUri()
        {
            //TODO: We should be able to specify a Pact file path
            //TODO: We should also provide a Pact Broker which allows us to obtain pact artifacts from build server etc

            return this;
        }*/

        public PactProvider GetMockProvider()
        {
            if (_pactProvider == null)
                throw new InvalidOperationException("Please define a Mock Service by calling MockService(...) before calling GetMockService()");

            return _pactProvider;
        }

        public void StartServer()
        {
            _pactProvider.Start();
        }

        public void StopServer()
        {
            _pactProvider.Stop();
            _pactProvider.Dispose();

            //TODO: Should we get from disk and append interactions

            //Check if file exists
                //If it does check if the interaction exists
                    //If it does overwrite interaction

            var pactFile = new PactFile
            {
                Provider = new PactParty { Name = _providerName },
                Consumer = new PactParty { Name = _consumerName },
                Interactions = new List<PactInteraction>
                {
                    _pactProvider.DescribeInteraction()
                }
            };

            var pactFileJson = JsonConvert.SerializeObject(pactFile, _jsonSettings);

            File.WriteAllText(PactFilePath, pactFileJson);
        }
    }
}
