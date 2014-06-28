using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Concord
{
    public class Pact
    {
        private string _consumerName;
        private string _providerName;
        private PactProvider _pactProvider;
        private string _pactFilePath = "./specs/pacts/";

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

        public Pact HonoursPactWith(string consumerName)
        {
            _consumerName = consumerName;

            var pactFilePath = Path.Combine(_pactFilePath, PactFileName);
            var pactFileJson = File.ReadAllText(pactFilePath);
            var jsonSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var pactFile = JsonConvert.DeserializeObject<PactFile>(pactFileJson, jsonSettings);
            pactFile.VerifyProvider();

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

        public PactProvider GetMockService()
        {
            if (_pactProvider == null)
                throw new InvalidOperationException("Please define a Mock Service by calling MockService(...) before calling GetMockService()");

            return _pactProvider;
        }
    }
}
