using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace Concord.Tests
{
    public class Tests
    {
        [Fact]
        public void ConsumerTest()
        {
            var pact = new Pact().ServiceConsumer("Source System")
                .HasPactWith("Event API")
                .MockService(1234);

            var pactServiceMock = pact.GetMockService();

            pactServiceMock.UponReceiving("A POST request with an event")
                .With(new PactProviderRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/events",
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    },
                    Body = new
                    {
                        EventId = Guid.NewGuid().ToString(),
                        Timestamp = DateTime.UtcNow.ToString("O"),
                        Host = "mymachine",
                        RemoteAddress = "",
                        EventType = "",
                        User = ""
                    }
                })
                .WillRespondWith(new PactProviderResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    },
                    Body = new { Test = "tester", Test2 = "Tester2" }
                });

            pact.StartServer();

            var client = new HttpClient();
            var response = client.GetAsync("http://localhost:1234/events");

            var content = response.Result.Content.ReadAsStringAsync().Result;
            var status = response.Result.StatusCode;

            Assert.Equal(HttpStatusCode.OK, status);

            pact.StopServer();
        }

        [Fact]
        public void ProviderTest()
        {
            //Create a test Owin API
            //Use Microsoft.Owin.Testing to spin up the Owin API
            
            var pact = new Pact().ServiceProvider("Event API")
                .HonoursPactWith("Source System");
        }
    }
}
