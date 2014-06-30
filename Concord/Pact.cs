using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Concord
{
    public class Pact //TODO: Should we split this into consumer and provider?
    {
        private string _consumerName;
        private string _providerName;
        private PactProvider _pactProvider;
        private string _pactFileDirectory = "./specs/pacts/";
        private readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
                                                                   {
                                                                       ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                                       NullValueHandling = NullValueHandling.Ignore,
                                                                       Formatting = Formatting.Indented
                                                                   };

        public string PactFilePath
        {
            get { return Path.Combine(_pactFileDirectory, PactFileName); }
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

        public Pact HonoursPactWith(string consumerName, TestServer server)
        {
            _consumerName = consumerName;

            var pactFileJson = File.ReadAllText(PactFilePath);
            var pactFile = JsonConvert.DeserializeObject<PactFile>(pactFileJson, JsonSettings);

            pactFile.VerifyProvider(server);

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
            var pactFile = new PactFile
            {
                Provider = new PactParty { Name = _providerName },
                Consumer = new PactParty { Name = _consumerName },
                Interactions = new List<PactInteraction>
                    {
                        _pactProvider.DescribeInteraction()
                    },
                Metadata = new { PactSpecificationVersion =  "1.0.0" }
            };

            var pactFileJson = JsonConvert.SerializeObject(pactFile, JsonSettings);

            File.WriteAllText(PactFilePath, pactFileJson);
        }
    }
}
