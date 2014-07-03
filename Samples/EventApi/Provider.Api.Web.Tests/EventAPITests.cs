using System;
using Microsoft.Owin.Testing;
using PactNet;
using Xunit;

namespace Provider.Api.Web.Tests
{
    public class EventApiTests : IDisposable
    {
        private readonly TestServer _testServer;

        public EventApiTests()
        {
            _testServer = TestServer.Create<Startup>();
        }

        public void Dispose()
        {
            //_testServer.Dispose();
        }

        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            new Pact().ServiceProvider("Event API")
                .HonoursPactWith("Consumer", _testServer.HttpClient);
        }
    }
}
