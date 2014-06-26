using System;

namespace Concord
{
    public class Pact
    {
        private string _consumerName;
        private string _serviceName;
        private PactService _pactService;

        public Pact()
        {
        }

        public Pact ServiceConsumer(string consumerName)
        {
            _consumerName = consumerName;

            return this;
        }

        public Pact HasPactWith(string serviceName)
        {
            _serviceName = serviceName;

            return this;
        }

        public Pact MockService(int port)
        {
            _pactService = new PactService(port);

            return this;
        }

        public PactService GetMockService()
        {
            if (_pactService == null)
                throw new InvalidOperationException("Please define a Mock Service by calling MockService(...) before calling GetMockService()");

            return _pactService;
        }
    }
}
