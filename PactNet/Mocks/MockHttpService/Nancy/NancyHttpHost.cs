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

        internal NancyHttpHost(Uri baseUri, string providerName, INancyBootstrapper bootstrapper)
        {
            var logFileName = String.Format("{0}_mock_service.log", providerName.ToLowerSnakeCase());
            LogProvider.LogFilePath = Path.Combine(Constants.DefaultLogFileDirectory.Replace(@"..\", String.Empty), logFileName);
            var logFilePath = Path.Combine(Constants.DefaultLogFileDirectory, logFileName);
            
            var logProvider = new LocalLogProvider(new List<ILocalLogMessageHandler> { new LocalLogNewRollingFileMessageHandler(logFilePath) });
            LogProvider.SetCurrentLogProvider(logProvider);

            _baseUri = baseUri;
            _bootstrapper = bootstrapper;
            _log = LogProvider.GetLogger(typeof(NancyHttpHost));
        }

        internal NancyHttpHost(Uri baseUri, string pactFileDirectory, string providerName)
            : this(baseUri, providerName, new MockProviderNancyBootstrapper(pactFileDirectory))
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