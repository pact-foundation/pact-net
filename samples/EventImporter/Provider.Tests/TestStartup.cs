using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.AspNetCore.Messaging;

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
            services.AddPactMessaging(options =>
            {
                options.BasePath = "/pact-messages";
                options.DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
            });

            this.inner.ConfigureServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePactMessaging();

            this.inner.Configure(app, env);
        }
    }
}
