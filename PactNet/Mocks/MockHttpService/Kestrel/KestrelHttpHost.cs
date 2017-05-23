#if USE_KESTREL

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;
using PactNet.Extensions;
using PactNet.Infrastructure;
using PactNet.Logging;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Kestrel;
using PactNet.Mocks.MockHttpService.Mappers;
using System;

namespace PactNet.Mocks.MockHttpService
{
    internal class HttpHost : IHttpHost
    {
        private readonly IWebHostBuilder _hostBuilder;
        private IWebHost _host;

        internal HttpHost(Uri baseUri, string providerName, PactConfig config, Action<IServiceCollection> overrides) : this(baseUri, providerName, config, false)
        {
            _hostBuilder.ConfigureServices(overrides);
        }

        internal HttpHost(Uri baseUri, string providerName, PactConfig config, bool bindOnAllAdapters)
        {
            string loggerName = LogProvider.CurrentLogProvider.AddLogger(config.LogDir, providerName.ToLowerSnakeCase(), "{0}_mock_service.log");

            config.LoggerName = loggerName;

            _hostBuilder = new WebHostBuilder()
                .UseUrls(baseUri.ToString())
                .UseKestrel()
                .ConfigureServices(services => services.AddSingleton(config))
                .UseStartup<Startup>();
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
            private readonly PactConfig _pactConfig;

            public Startup(PactConfig pactConfig)
            {
                _pactConfig = pactConfig;
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseMiddleware<KestrelHandler>();
            }

            public void ConfigureServices(IServiceCollection services)
            {
                services
                    .AddTransient<IProviderServiceRequestMapper, ProviderServiceRequestMapper>()
                    .AddTransient<IProviderServiceRequestComparer, ProviderServiceRequestComparer>()
                    .AddTransient<IResponseMapper, ResponseMapper>()
                    .AddTransient<IMockProviderRequestHandler, MockProviderRequestHandler>()
                    .AddTransient<IMockProviderAdminRequestHandler, MockProviderAdminRequestHandler>()
                    .AddSingleton<IMockProviderRepository, MockProviderRepository>()
                    .AddTransient<IFileSystem, FileSystem>()
                    .AddSingleton(provider => LogProvider.GetLogger(_pactConfig.LoggerName));
            }
        }
    }
}

#endif