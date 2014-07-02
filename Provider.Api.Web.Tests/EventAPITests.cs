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
            var testServer = TestServer.Create<Startup>();

            new Pact().ServiceProvider("Event API")
                .HonoursPactWith("Consumer", testServer.HttpClient);

            testServer.Dispose();
        }
    }
}
