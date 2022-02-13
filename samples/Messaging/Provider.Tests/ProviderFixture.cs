using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.AspNetCore.Messaging;
using Xunit;

namespace Provider.Tests
{
    /// <summary>
    /// Fixture for running the Pact messaging mock server
    /// </summary>
    public class ProviderFixture : IAsyncLifetime
    {
        private IAsyncDisposable server;

        /// <summary>
        /// Address of the mock server which drives Pact messaging requests
        /// </summary>
        public Uri ServerUri => new Uri("http://localhost:9333/");

        /// <summary>
        /// Base path of the Pact messages endpoint
        /// </summary>
        public string BasePath => "/";

        /// <summary>
        /// Called immediately after the class has been created, before it is used.
        /// </summary>
        public async Task InitializeAsync()
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder();

            builder.Services.AddPactMessaging(options =>
            {
                options.DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };

                options.BasePath = this.BasePath;
            });

            WebApplication app = builder.Build();

            app.Urls.Add(this.ServerUri.AbsoluteUri);
            app.UsePactMessaging();

            await app.StartAsync();

            this.server = app;
        }

        /// <summary>
        /// Called when an object is no longer needed. Called just before <see cref="M:System.IDisposable.Dispose" />
        /// if the class also implements that.
        /// </summary>
        public async Task DisposeAsync()
        {
            await this.server.DisposeAsync();
        }
    }
}
