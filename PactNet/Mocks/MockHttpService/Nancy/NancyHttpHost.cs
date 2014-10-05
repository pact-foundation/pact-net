using System;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using PactNet.Mocks.MockHttpService.Configuration;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class NancyHttpHost : IHttpHost
    {
        private readonly Uri _baseUri;
        private readonly INancyBootstrapper _bootstrapper;
        private NancyHost _host;

        internal NancyHttpHost(Uri baseUri, INancyBootstrapper bootstrapper)
        {
            _baseUri = baseUri;
            _bootstrapper = bootstrapper;
        }

        public NancyHttpHost(Uri baseUri)
            : this(baseUri, new MockProviderNancyBootstrapper())
        {
            _baseUri = baseUri;
        }

        public void Start()
        {
            Stop();
            _host = new NancyHost(_bootstrapper, NancyConfig.HostConfiguration, _baseUri);
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