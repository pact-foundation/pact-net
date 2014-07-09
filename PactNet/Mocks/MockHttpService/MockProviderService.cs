using System;
using System.Collections.Generic;
using Nancy.Hosting.Self;
using PactNet.Mocks.MockHttpService.Configuration;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Models;

namespace PactNet.Mocks.MockHttpService
{
    public class MockProviderService : IMockProviderService
    {
        private readonly string _baseUri;
        private NancyHost _host;

        private string _providerState;
        private string _description;
        private PactProviderServiceRequest _request;
        private PactProviderServiceResponse _response;

        private readonly IList<PactServiceInteraction> _interactions;
        public IEnumerable<PactInteraction> Interactions
        {
            get { return _interactions; }
        }

        public MockProviderService(int port)
        {
            _baseUri = String.Format("http://localhost:{0}", port);
            _interactions = new List<PactServiceInteraction>();
        }

        public IMockProviderService Given(string providerState)
        {
            _providerState = providerState;

            return this;
        }

        public IMockProviderService UponReceiving(string description)
        {
            _description = description;

            return this;
        }

        public IMockProviderService With(PactProviderServiceRequest request)
        {
            _request = request;
            
            return this;
        }

        public IMockProviderService WillRespondWith(PactProviderServiceResponse response)
        {
            _response = response;
            
            return this;
        }

        public void Register()
        {
            var interaction = new PactServiceInteraction
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

            MockProviderNancyRequestDispatcher.Set(interaction.Request, interaction.Response);
        }

        public void Start()
        {
            MockProviderNancyRequestDispatcher.Reset();

            _host = new NancyHost(new MockProviderNancyBootstrapper(), NancyConfig.HostConfiguration, new Uri(_baseUri));

            _host.Start();
        }

        public void Stop()
        {
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                MockProviderNancyRequestDispatcher.Reset();
            }
        }
    }
}
