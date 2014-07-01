using Microsoft.Owin.Testing;
using Xunit;

namespace Concord.Api.Web.Tests
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
