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
        private readonly Func<Uri, NancyHost> _nancyHostFactory;

        private NancyHost _host;

        private string _providerState;
        private string _description;
        private PactProviderServiceRequest _request;
        private PactProviderServiceResponse _response;

        private IList<PactServiceInteraction> _interactions;
        public IEnumerable<PactInteraction> Interactions
        {
            get { return _interactions; }
        }

        public string BaseUri { get; private set; }

        [Obsolete("For testing only.")]
        public MockProviderService(Func<Uri, NancyHost> nancyHostFactory, int port)
        {
            _nancyHostFactory = nancyHostFactory;
            BaseUri = String.Format("http://localhost:{0}", port);
        }

        public MockProviderService(int port)
            : this(baseUri => new NancyHost(new MockProviderNancyBootstrapper(), NancyConfig.HostConfiguration, baseUri), port)
        {
        }

        public IMockProviderService Given(string providerState)
        {
            if (String.IsNullOrEmpty(providerState))
            {
                throw new ArgumentException("Please supply a non null or empty providerState");
            }

            _providerState = providerState;

            return this;
        }

        public IMockProviderService UponReceiving(string description)
        {
            if (String.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Please supply a non null or empty description");
            }

            _description = description;

            return this;
        }

        public IMockProviderService With(PactProviderServiceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentException("Please supply a non null request");
            }

            _request = request;
            
            return this;
        }

        public IMockProviderService WillRespondWith(PactProviderServiceResponse response)
        {
            if (response == null)
            {
                throw new ArgumentException("Please supply a non null response");
            }

            _response = response;
            
            return this;
        }

        public void RegisterInteraction()
        {
            if (String.IsNullOrEmpty(_description))
            {
                throw new InvalidOperationException("description has not been set, please supply using the UponReceiving method.");
            }

            if (_request == null)
            {
                throw new InvalidOperationException("request has not been set, please supply using the With method.");
            }

            if (_response == null)
            {
                throw new InvalidOperationException("response has not been set, please supply using the WillRespondWith method.");
            }

            var interaction = new PactServiceInteraction
            {
                ProviderState = _providerState,
                Description = _description,
                Request = _request,
                Response = _response
            };

            _providerState = null;
            _description = null;
            _request = null;
            _response = null;

            _interactions = _interactions ?? new List<PactServiceInteraction>();
            _interactions.Add(interaction);

            MockProviderNancyRequestDispatcher.Set(interaction.Request, interaction.Response); //TODO: Can't test this nicely
        }

        public void Start() //TODO: Can't test this nicely
        {
            MockProviderNancyRequestDispatcher.Reset();

            _host = _nancyHostFactory(new Uri(BaseUri));
            _host.Start();
        }

        public void Stop() //TODO: Can't test this nicely
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
