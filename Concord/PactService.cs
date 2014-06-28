using System;
using Nancy.Hosting.Self;

namespace Concord
{
    public class PactService : IDisposable
    {
        private NancyHost _host;
        private readonly string _uri;
        private string _description;
        private PactServiceRequest _request;
        private PactServiceResponse _response;

        public PactService(int port)
        {
            _uri = String.Format("http://localhost:{0}", port);
        }

        public PactService UponReceiving(string description)
        {
            _description = description;

            return this;
        }

        public PactService With(PactServiceRequest request)
        {
            _request = request;

            return this;
        }

        public PactService WillRespondWith(PactServiceResponse response)
        {
            _response = response;

            return this;
        }

        public void Start()
        {
            PactServiceNancyModule.Set(_request, _response);

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
