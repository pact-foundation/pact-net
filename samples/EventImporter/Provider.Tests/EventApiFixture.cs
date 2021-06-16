using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
                              })
                              .Build();

            this.server.Start();
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            this.server.Dispose();
        }
    }
}
