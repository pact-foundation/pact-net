using System;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Configuration;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal class NancyHttpHost : IHttpHost
    {
        private readonly Uri _baseUri;
        private readonly INancyBootstrapper _bootstrapper;
        private readonly ILog _log;
        private readonly PactConfig _config;
        private NancyHost _host;

        internal NancyHttpHost(Uri baseUri, PactConfig config, INancyBootstrapper bootstrapper)
        {
            _baseUri = baseUri;
            _bootstrapper = bootstrapper;
            _log = LogProvider.GetLogger(config.LoggerName);
            _config = config;
        }

        internal NancyHttpHost(Uri baseUri, string providerName, PactConfig config)
        {
            var loggerName = LogProvider.CurrentLogProvider.AddLogger(config.LogDir, providerName.ToLowerSnakeCase(), "{0}_mock_service.log");
            config.LoggerName = loggerName;

            _baseUri = baseUri;
            _bootstrapper = new MockProviderNancyBootstrapper(config);
            _log = LogProvider.GetLogger(config.LoggerName);
            _config = config;
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
                Dispose(_host);
                _host = null;
                _log.InfoFormat("Stopped {0}", _baseUri.OriginalString);

                LogProvider.CurrentLogProvider.RemoveLogger(_config.LoggerName);
            }
        }

        private void Dispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}