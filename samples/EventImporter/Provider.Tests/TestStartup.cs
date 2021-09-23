using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PactNet.AspNetCore.Messaging;
using PactNet.AspNetCore.Messaging.Options;

namespace Provider.Tests
{
    public class TestStartup
    {
        private readonly Startup inner;
        private readonly IConfiguration configuration;

        public TestStartup(IConfiguration configuration)
        {
            this.configuration = configuration;

            this.inner = new Startup(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MessagingVerifierOptions>(configuration.GetSection("PactMessageMiddleware"));

            this.inner.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMessaging();

            this.inner.Configure(app, env);
        }
    }
}
