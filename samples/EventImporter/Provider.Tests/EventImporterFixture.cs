using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PactNet.AspNetCore.Messaging.Options;
using PactNet.Native;

namespace Provider.Tests
{
    public class EventImporterFixture : IDisposable
    {
        private readonly IHost server;

        public Uri ServerUri { get; }


        public EventImporterFixture()
        {
            this.ServerUri = new Uri("http://localhost:9333/");

            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(ServerUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                              })
                              .Build();

            this.server.Start();
        }

        /// <summary>
        /// Get the options for the middleware
        /// </summary>
        /// <returns>The options</returns>
        public MessagingVerifierOptions GetOptions()
        {
            return this.server.Services.GetService<IOptions<MessagingVerifierOptions>>()?.Value;
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
