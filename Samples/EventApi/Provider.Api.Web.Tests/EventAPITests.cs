using Microsoft.Owin.Testing;
using PactNet;
using Xunit;

namespace Provider.Api.Web.Tests
{
    public class EventApiTests
    {
        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            //Arrange
            var pactVerifier = new PactVerifier();

            pactVerifier
                .ProviderStatesFor("Consumer")
                .ProviderState("There are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'",
                    setUp: InsertEventsIntoDatabase)
                .ProviderState("There is an event with id '83f9262f-28f1-4703-ab1a-8cfd9e8249c9'",
                    setUp: InsertEventIntoDatabase)
                .ProviderState("There is one event with type 'DetailsView'",
                    setUp: EnsureOneDetailsViewEventExists);

            //Act / Assert
            using (var testServer = TestServer.Create<Startup>())
            {
                pactVerifier
                   .ServiceProvider("Event API", testServer.HttpClient)
                   .HonoursPactWith("Consumer")
                   .PactUri("../../../Consumer.Tests/pacts/consumer-event_api.json")
                   .Verify();
            }
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
    }
}
