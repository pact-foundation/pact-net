using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Provider.Orders;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class ProviderTests : IDisposable
    {
        private static readonly Uri ProviderUri = new("http://localhost:5000");

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        private readonly IHost server;
        private readonly PactVerifier verifier;

        public ProviderTests(ITestOutputHelper output)
        {
            this.server = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(webBuilder =>
                              {
                                  webBuilder.UseUrls(ProviderUri.ToString());
                                  webBuilder.UseStartup<TestStartup>();
                              })
                              .Build();

            this.server.Start();
            
            this.verifier = new PactVerifier("Orders API", new PactVerifierConfig
            {
                LogLevel = PactLogLevel.Debug,
                Outputters = new List<IOutput>
                {
                    new XunitOutput(output)
                }
            });
        }

        public void Dispose()
        {
            this.server.Dispose();
            this.verifier.Dispose();
        }

        [Fact]
        public void Verify()
        {
            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "Consumer.Tests",
                                           "pacts",
                                           "Fulfilment API-Orders API.json");

            this.verifier
                .WithHttpEndpoint(ProviderUri)
                .WithMessages(scenarios =>
                {
                    scenarios.Add("an event indicating that an order has been created", () => new OrderCreatedEvent(1));
                }, Options)
                .WithFileSource(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(ProviderUri, "/provider-states"))
                .Verify();
        }
    }
}
