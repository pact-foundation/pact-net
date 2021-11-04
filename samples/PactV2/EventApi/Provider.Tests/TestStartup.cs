using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PactNet.AspNetCore.ProviderState;

namespace Provider.Tests
{
    public class TestStartup
    {
        private readonly Startup inner;

        public TestStartup(IConfiguration configuration)
        {
            this.inner = new Startup(configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPactProviderState(options =>
            {
                options.RouteProviderState = "/provider-states";
            });

            this.inner.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePactProviderStates()
               .UseMiddleware<AuthorizationTokenReplacementMiddleware>();

            this.inner.Configure(app, env);
        }
    }
}
