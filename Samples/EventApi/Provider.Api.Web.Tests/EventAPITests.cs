using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Testing;
using PactNet;
using PactNet.Reporters.Outputters;
using Xunit;

namespace Provider.Api.Web.Tests
{
    public class EventApiTests : IDisposable
    {
        private TestServer _server;

        [Fact]
        public async Task EnsureEventApiHonoursPactWithConsumer()
        {
            //Arrange
            var outputter = new CustomOutputter();
            var config = new PactVerifierConfig();
            config.ReportOutputters.Add(outputter);
            IPactVerifier pactVerifier = new PactVerifier(() => { }, () => { }, config);

            pactVerifier
                .ProviderState(
                    "there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'",
                    setUp: InsertEventsIntoDatabase)
                .ProviderState("there is an event with id '83f9262f-28f1-4703-ab1a-8cfd9e8249c9'",
                    setUp: InsertEventIntoDatabase)
                .ProviderState("there is one event with type 'DetailsView'",
                    setUp: EnsureOneDetailsViewEventExists);

            _server = TestServer.Create(app =>
            {
                app.Use(typeof(AuthorizationTokenReplacementMiddleware), app.CreateDataProtector(typeof(OAuthAuthorizationServerMiddleware).Namespace, "Access_Token", "v1"));
                var apiStartup = new Startup();
                apiStartup.Configuration(app);
            });

            //Act / Assert
            await pactVerifier
                   .ServiceProvider("Event API", _server.HttpClient)
                   .HonoursPactWith("Consumer")
                   .PactUri("../../../Consumer.Tests/pacts/consumer-event_api.json")
                   .Verify();

            // Verify that verifaction log is also sent to additional reporters defined in the config
            Assert.Contains("Verifying a Pact between Consumer and Event API", outputter.Output);
        }

        private void EnsureOneDetailsViewEventExists()
        {
            //Logic to check and insert a details view event
        }

        private void InsertEventsIntoDatabase()
        {
            //Logic to do database inserts or events api calls to create data
        }

        private void InsertEventIntoDatabase()
        {
            //Logic to do database inserts for event with id 83F9262F-28F1-4703-AB1A-8CFD9E8249C9
        }


        public virtual void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }

        private class CustomOutputter : IReportOutputter
        {
            public string Output { get; private set; }

            public void Write(string report)
            {
                Output += report;
            }
        }
    }
}
