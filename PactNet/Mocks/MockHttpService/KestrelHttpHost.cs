#if NETSTANDARD1_6

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Nancy.Bootstrapper;
using Nancy.Owin;
using PactNet.Extensions;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Nancy;
using System;
using System.IO;

namespace PactNet.Mocks.MockHttpService
{
    internal class HttpHost : IHttpHost
    {
        private readonly IWebHostBuilder _hostBuilder;
        private IWebHost _host;

        internal HttpHost(Uri baseUri, string providerName, PactConfig config, INancyBootstrapper bootstrapper) :
            this(baseUri, providerName, config, false, bootstrapper)
        {
        }

        internal HttpHost(Uri baseUri, string providerName, PactConfig config, bool bindOnAllAdapters, INancyBootstrapper bootstrapper = null)
        {
            string loggerName = LogProvider.CurrentLogProvider.AddLogger(config.LogDir, providerName.ToLowerSnakeCase(), "{0}_mock_service.log");

            config.LoggerName = loggerName;
            bootstrapper = bootstrapper ?? new MockProviderNancyBootstrapper(config);

            _hostBuilder = new WebHostBuilder()
                .UseUrls(baseUri.ToString())
                .UseKestrel()
                .UseStartup<Startup>()
                .ConfigureServices(services => services.AddSingleton(bootstrapper));
        }

        public void Start()
        {
            Stop();

            _host = _hostBuilder.Build();
            _host.Start();
        }

        public void Stop()
        {
            if (_host == null)
            {
                return;
            }

            _host.Services.GetService<IApplicationLifetime>().StopApplication();
            _host.Dispose();
            _host = null;
        }

        private class Startup
        {
            public void Configure(IApplicationBuilder appBuilder, INancyBootstrapper bootstrapper)
            {
                appBuilder.UseOwin(app => app.UseNancy(opt => opt.Bootstrapper = bootstrapper));
            }
        }
    }
}

#endif