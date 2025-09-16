using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Interop;

namespace GrpcGreeter.Tests;

public class ServerFixture : IDisposable
{
    public readonly Uri ProviderUri = new("http://localhost:5000");
    private readonly IHost server;

    public ServerFixture()
    {
        PactLogLevel.Information.InitialiseLogging();
        this.server = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls(this.ProviderUri.ToString());
                webBuilder.UseStartup<Startup>();
            })
            .Build();

        this.server.Start();
    }

    public void Dispose() => this.server?.Dispose();
}
