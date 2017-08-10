using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Owin.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Api.Web.Tests
{
    public class EventApiTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public EventApiTests(ITestOutputHelper output)
        {
            _output = output;
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
                    new XUnitOutput(_output)
                }
            };
            
            using (WebApp.Start<TestStartup>(serviceUri))
            {
                //Act / Assert
                IPactVerifier pactVerifier = new PactVerifier(config);
                pactVerifier
                    .ProviderState($"{serviceUri}/provider-states")
                    .ServiceProvider("Event API", serviceUri)
                    .HonoursPactWith("Event API Consumer")
                    .PactUri(@"..\..\..\..\Consumer.Tests\pacts\event_api_consumer-event_api.json")
                    .Verify();
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
