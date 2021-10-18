using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PactNet.AspNetCore.ProviderState;
using Provider.Controllers;

namespace Provider.Tests
{
    public class EventApiFixture : IDisposable
    {
        private readonly IHost server;

        public Uri ServerUri { get; }

        public EventApiFixture()
        {
            this.ServerUri = new Uri("http://localhost:9222");

            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(this.ServerUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                                  webBuilder.ConfigureServices(services => services.AddSingleton<IEventRepository, InMemoryEventRepository>());
                              })
                              .Build();

            this.server.Start();
        }

        /// <summary>
        /// Get the options for the middleware
        /// </summary>
        /// <returns>The options</returns>
        public ProviderStateOptions GetOptions()
        {
            return this.server.Services.GetService<IOptions<ProviderStateOptions>>()?.Value;
        }

        public IEventRepository GetFakeEventRepository()
        {
            return this.server.Services.GetService<IEventRepository>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.server.Dispose();
        }
    }
}
