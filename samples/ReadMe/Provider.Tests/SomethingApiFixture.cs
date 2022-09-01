using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ReadMe.Provider.Tests
{
    public class SomethingApiFixture : IDisposable
    {
        private readonly IHost server;
        public Uri ServerUri { get; }

        public SomethingApiFixture()
        {
            ServerUri = new Uri("http://localhost:9223");
            server = Host.CreateDefaultBuilder()
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseUrls(ServerUri.ToString());
                                webBuilder.UseStartup<TestStartup>();
                            })
                            .Build();
            server.Start();
        }

        public void Dispose()
        {
            server.Dispose();

            // Clean out leftover data
            foreach (var dataFile in Directory.GetFiles(Path.Combine("..", "..", "..", "data")))
            {
                File.Delete(dataFile);
            }
        }
    }
}
