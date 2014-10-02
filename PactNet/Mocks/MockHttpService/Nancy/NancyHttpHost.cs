using System;
using Nancy.Hosting.Self;
using PactNet.Mocks.MockHttpService.Configuration;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class NancyHttpHost : IHttpHost
    {
        private readonly Uri _baseUri;
        private NancyHost _host;

        public NancyHttpHost(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public void Start()
        {
            Stop();
            _host = new NancyHost(new MockProviderNancyBootstrapper(), NancyConfig.HostConfiguration, _baseUri);
            _host.Start();
        }

        public void Stop()
        {
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                _host = null;
            }
        }
    }
}