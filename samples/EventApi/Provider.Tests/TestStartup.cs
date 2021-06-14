using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            this.inner.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ProviderStateMiddleware>()
               .UseMiddleware<AuthorizationTokenReplacementMiddleware>();

            this.inner.Configure(app, env);
        }
    }
}
