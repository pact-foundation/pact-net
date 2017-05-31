using System;
using Microsoft.Owin.Hosting;
using PactNet;
using Xunit;

namespace Provider.Api.Web.Tests
{
    public class EventApiTests : IDisposable
    {
        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            //Arrange
            const string serviceUri = "http://localhost:9222";
            //var outputter = new CustomOutputter();
            var config = new PactVerifierConfig();

            //TODO: What do we want to do about this
            //config.ReportOutputters.Add(outputter);
            
            using (WebApp.Start<TestStartup>(serviceUri))
            {
                //Act / Assert
                IPactVerifier pactVerifier = new PactVerifier(config);
                pactVerifier
                    .ProviderState(new Uri($"{serviceUri}/provider-states"))
                    .ServiceProvider("Event API", new Uri(serviceUri))
                    .HonoursPactWith("Event API Consumer")
                    .PactUri("../../../Consumer.Tests/pacts/event_api_consumer-event_api.json") //TODO: What to do when we want to talk to multiple brokers
                    .Verify();

                // Verify that verifaction log is also sent to additional reporters defined in the config
                //Assert.Contains("Verifying a Pact between Consumer and Event API", outputter.Output);
            }
        }

        public virtual void Dispose()
        {
        }

        /*private class CustomOutputter : IReportOutputter
        {
            public string Output { get; private set; }

            public void Write(string report)
            {
                Output += report;
            }
        }*/
    }
}
