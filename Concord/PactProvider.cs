using System;
using Nancy.Hosting.Self;

namespace Concord
{
    public class PactProvider : IDisposable
    {
        private NancyHost _host;
        private readonly string _uri;
        private string _description;
        private PactProviderRequest _request;
        private PactProviderResponse _response;

        public PactProvider(int port)
        {
            _uri = String.Format("http://localhost:{0}", port);
        }

        public PactProvider UponReceiving(string description)
        {
            _description = description;

            return this;
        }

        public PactProvider With(PactProviderRequest request)
        {
            _request = request;

            return this;
        }

        public PactProvider WillRespondWith(PactProviderResponse response)
        {
            _response = response;

            return this;
        }

        public void Start()
        {
            PactProviderNancyModule.Set(_request, _response);

            var hostConfig = new HostConfiguration { UrlReservations = { CreateAutomatically = true }, AllowChunkedEncoding = false };
            _host = new NancyHost(hostConfig, new Uri(_uri));

            _host.Start();
        }

        public void Stop()
        {
            _host.Stop();
        }

        public void Dispose()
        {
            if(_host != null)
                _host.Dispose();
        }
    }
}
