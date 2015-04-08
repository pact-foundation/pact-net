using System;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using PactNet.Mocks.MockHttpService.Configuration;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal class NancyHttpHost : IHttpHost
    {
        private readonly Uri _baseUri;
        private readonly INancyBootstrapper _bootstrapper;
        private readonly ILogger _logger;
        private NancyHost _host;

        internal NancyHttpHost(Uri baseUri, INancyBootstrapper bootstrapper)
        {
            _baseUri = baseUri;
            _bootstrapper = bootstrapper;
            _logger = new Logger();
        }

        public NancyHttpHost(Uri baseUri, string pactFileDirectory)
            : this(baseUri, new MockProviderNancyBootstrapper(pactFileDirectory))
        {
            _baseUri = baseUri;
        }

        public void Start()
        {
            Stop();
            _host = new NancyHost(_bootstrapper, NancyConfig.HostConfiguration, _baseUri);
            _host.Start();
            _logger.Log("Started " + _baseUri.OriginalString);
        }

        public void Stop()
        {
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                _host = null;
                _logger.Log("Stopped " + _baseUri.OriginalString);
            }
        }
    }
}