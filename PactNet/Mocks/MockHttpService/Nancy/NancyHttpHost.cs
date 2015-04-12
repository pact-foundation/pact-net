using System;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Configuration;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal class NancyHttpHost : IHttpHost
    {
        private readonly Uri _baseUri;
        private readonly INancyBootstrapper _bootstrapper;
        private readonly ILog _log;
        private NancyHost _host;

        internal NancyHttpHost(Uri baseUri, INancyBootstrapper bootstrapper)
        {
            _baseUri = baseUri;
            _bootstrapper = bootstrapper;
            _log = LogProvider.GetLogger(typeof(NancyHttpHost));
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
            _log.InfoFormat("Started {0}", _baseUri.OriginalString);
        }

        public void Stop()
        {
            if (_host != null)
            {
                _host.Stop();
                _host.Dispose();
                _host = null;
                _log.InfoFormat("Stopped {0}", _baseUri.OriginalString);
            }
        }
    }
}