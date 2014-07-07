using System;
using System.Collections.Generic;
using Nancy.Hosting.Self;
using PactNet.Configuration.Nancy;

namespace PactNet.Consumer.Mocks.MockService
{
    public class MockProviderService : IMockProviderService
    {
        private readonly string _baseUri;
        private NancyHost _host;

        private string _providerState;
        private string _description;
        private PactProviderRequest _request;
        private PactProviderResponse _response;

        private readonly IList<PactInteraction> _interactions;
        public IEnumerable<PactInteraction> Interactions
        {
            get { return _interactions; }
        }

        public MockProviderService(int port)
        {
            _baseUri = String.Format("http://localhost:{0}", port);
            _interactions = new List<PactInteraction>();
        }

        public IMockProvider Given(string providerState)
        {
            _providerState = providerState;

            return this;
        }

        public IMockProvider UponReceiving(string description)
        {
            _description = description;

            return this;
        }

        public IMockProvider With(PactProviderRequest request)
        {
            _request = request;
            
            return this;
        }

        public IMockProvider WillRespondWith(PactProviderResponse response)
        {
            _response = response;
            
            return this;
        }

        public void Register()
        {
            var interaction = new PactInteraction
            {
                Description = _description,
                ProviderState = _providerState,
                Request = _request,
                Response = _response
            };

            _providerState = null;
            _description = null;
            _request = null;
            _response = null;

            _interactions.Add(interaction);

            //TODO: Register for all tests instead?
            PactNancyRequestDispatcher.Set(interaction.Request);
            PactNancyRequestDispatcher.Set(interaction.Response);
        }

        public void Start()
        {
            PactNancyRequestDispatcher.Reset();

            _host = new NancyHost(new PactNancyBootstrapper(), NancyConfig.HostConfiguration, new Uri(_baseUri));
            _host.Start();
        }

        public void Stop()
        {
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                PactNancyRequestDispatcher.Reset(); //TODO: Can potentially get rid of this
            }
        }
    }
}
