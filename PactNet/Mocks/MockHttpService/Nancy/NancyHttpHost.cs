using System;
using System.Globalization;
using System.IO;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Configuration;
using Serilog;

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
            var logFileNameFormat = String.Format("{0}_mock_service-{{Date}}.log", providerName.ToLowerSnakeCase());
            var logFilePathFormat = Path.Combine(Constants.DefaultLogFileDirectory, logFileNameFormat);
            LogProvider.LogFilePath = Path.Combine(Constants.DefaultLogFileDirectory.Replace(@"..\", String.Empty), logFileNameFormat.Replace("{Date}", DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture)));

            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .RollingFile(logFilePathFormat)
                .CreateLogger();

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