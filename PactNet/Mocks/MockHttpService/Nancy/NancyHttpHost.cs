using System;
using System.Collections.Generic;
using System.IO;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using PactNet.Extensions;
using PactNet.Infrastructure.Logging;
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

        internal NancyHttpHost(Uri baseUri, string providerName, INancyBootstrapper bootstrapper, string logDir)
        {
            var logFileName = String.Format("{0}_mock_service.log", providerName.ToLowerSnakeCase());
            var logFilePath = Path.Combine(logDir, logFileName);
            LogProvider.LogFilePath = Path.GetFullPath(logFilePath);
            
            var logProvider = new LocalLogProvider(new List<ILocalLogMessageHandler> { new LocalRollingLogFileMessageHandler(logFilePath) });
            LogProvider.SetCurrentLogProvider(logProvider);

            _baseUri = baseUri;
            _bootstrapper = bootstrapper;
            _log = LogProvider.GetLogger(typeof(NancyHttpHost));
        }

        internal NancyHttpHost(Uri baseUri, string providerName, PactConfig config)
            : this(baseUri, providerName, new MockProviderNancyBootstrapper(config), config.LogDir)
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
                Dispose(_host);
                _host = null;
                _log.InfoFormat("Stopped {0}", _baseUri.OriginalString);

                Dispose(LogProvider.CurrentLogProvider);
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