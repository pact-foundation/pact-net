using Microsoft.Owin.Testing;
using Xunit;

namespace Concord.Api.Web.Tests
{
    public class Tests
    {
        [Fact]
        public void ProviderTest()
        {
            var testServer = TestServer.Create<Startup>();

            var pact = new Pact().ServiceProvider("Event API")
                .HonoursPactWith("Source System", testServer.HttpClient);

            testServer.Dispose();
        }
    }
}
