using System;
using System.Collections.Generic;
using Xunit;

namespace Concord.Tests
{
    public class Tests
    {
        [Fact]
        public void Test()
        {
            var pact = new Pact().ServiceConsumer("Source System")
                .HasPactWith("Event API")
                .MockService(1234);

            var pactServiceMock = pact.GetMockService();

            pactServiceMock.UponReceiving("A POST request with an event")
                .With(new PactServiceRequest
                {
                    Method = HttpVerb.Post,
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
                .WillRespondWith(new PactServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" }
                    },
                    Body = new {}
                });

            pactServiceMock.Start();



            pactServiceMock.Stop();
        }
    }
}
