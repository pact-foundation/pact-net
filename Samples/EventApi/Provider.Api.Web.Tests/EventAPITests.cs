using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Owin.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Native;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Api.Web.Tests
{
    public class EventApiTests : IDisposable
    {
        private readonly ITestOutputHelper output;

        public EventApiTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            //Arrange
            const string serviceUri = "http://localhost:9222";
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(this.output)
                }
            };

            using (WebApp.Start<TestStartup>(serviceUri))
            {
                string pactPath = Path.Combine("..",
                                               "..",
                                               "..",
                                               "..",
                                               "Consumer.Tests",
                                               "pacts",
                                               "Event API Consumer-Event API.json");

                //Act / Assert
                IPactVerifier pactVerifier = new PactVerifier(config);
                pactVerifier
                    .ProviderState($"{serviceUri}/provider-states")
                    .ServiceProvider("Event API", new Uri(serviceUri))
                    .HonoursPactWith("Event API Consumer")
                    .PactFile(new FileInfo(pactPath))
                    .Verify();
            }
        }

        public virtual void Dispose()
        {
        }
    }
}