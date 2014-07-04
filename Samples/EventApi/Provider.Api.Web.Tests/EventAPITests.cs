using System;
using System.Collections.Generic;
using Microsoft.Owin.Testing;
using PactNet;
using PactNet.Provider;
using Xunit;

namespace Provider.Api.Web.Tests
{
    public class EventApiTests : IDisposable
    {
        private readonly TestServer _testServer;
        private readonly IPactProvider _pact;

        public EventApiTests()
        {
            _testServer = TestServer.Create<Startup>();

            _pact = new Pact()
                .ProviderStatesFor("Consumer", 
                new Dictionary<string, Action>
                {
                    { "There are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'", InsertEventsIntoDatabaseIfTheyDontExist }
                });
        }

        public void Dispose()
        {
            _testServer.Dispose();
        }

        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            _pact.ServiceProvider("Event API", _testServer.HttpClient)
                .HonoursPactWith("Consumer")
                .PactUri("../../../Consumer.Tests/pacts/consumer-event_api.json")
                .Execute();
        }

        private void InsertEventsIntoDatabaseIfTheyDontExist()
        {
            //Logic to do database inserts or events api calls to create data
        }
    }
}
