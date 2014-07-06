using System;
using Nancy.Hosting.Self;

namespace PactNet.Consumer.Mocks.MockService
{
    public class MockProviderService : IMockProviderService
    {
        private readonly string _baseUri;
        private string _providerState;
        private string _description;
        private PactProviderRequest _request;
        private PactProviderResponse _response;
        private NancyHost _host;

        public MockProviderService(int port)
        {
            _baseUri = String.Format("http://localhost:{0}", port);
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
            PactNancyRequestDispatcher.Set(request);

            return this;
        }

        public IMockProvider WillRespondWith(PactProviderResponse response)
        {
            _response = response;
            PactNancyRequestDispatcher.Set(response);

            return this;
        }

        public void Start()
        {
            PactNancyRequestDispatcher.Reset();

            var hostConfig = new HostConfiguration { UrlReservations = { CreateAutomatically = true }, AllowChunkedEncoding = false };
            _host = new NancyHost(new PactNancyBootstrapper(), hostConfig, new Uri(_baseUri));

            _host.Start();
        }

        public void Stop()
        {
            _host.Stop();

            PactNancyRequestDispatcher.Reset();
        }

        public PactInteraction DescribeInteraction()
        {
            return new PactInteraction
            {
                Description = _description,
                ProviderState = _providerState,
                Request = _request,
                Response = _response
            };
        }

        public void Dispose()
        {
            if(_host != null)
                _host.Dispose();
        }
    }
}
