using System;
using Nancy.Hosting.Self;

namespace Concord
{
    public class PactService : IDisposable
    {
        private NancyHost _host;
        private string _description;
        private PactServiceRequest _request;
        private PactServiceResponse _response;

        public PactService(int port)
        {
            var hostConfig = new HostConfiguration
            {
                UrlReservations = { CreateAutomatically = true }
            };

            var uri = String.Format("http://localhost:{0}", port);

            _host = new NancyHost(hostConfig, new Uri(uri));
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
            //Register the built module

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
