using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Consumer.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet;
using PactNet.Matchers;
using PactNet.Native;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class EventsApiConsumerTests
    {
        private const string Token = "SomeValidAuthToken";

        private readonly IPactBuilderV3 pact;

        public EventsApiConsumerTests(ITestOutputHelper output)
        {
            var config = new PactConfig
            {
                LogDir = "../../../logs/",
                PactDir = "../../../pacts/",
                Outputters = new[]
                {
                    new XUnitOutput(output)
                },
                DefaultJsonSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            IPactV3 pact = Pact.V3("Event API Consumer", "Event API", config);
            this.pact = pact.UsingNativeBackend();
        }

        [Fact]
        public async Task GetEventsByType_WhenOneEventWithTheTypeExists_ReturnsEvent()
        {
            //Arrange
            const string eventType = "DetailsView";

            this.pact
                .UponReceiving($"a request to retrieve events with type '{eventType}'")
                    .Given($"at least one event of type eventType exists", new Dictionary<string, string>{{"eventType", eventType}})
                    .WithRequest(HttpMethod.Get, "/events")
                    .WithQuery("type", eventType)
                    .WithHeader("Accept", "application/json")
                    .WithHeader("Authorization", $"Bearer {Token}")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new[]
                    {
                        new
                        {
                            eventType
                        }
                    });

            await this.pact.VerifyAsync(async ctx =>
            {
                var client = new EventsApiClient(ctx.MockServerUri, Token);

                IEnumerable<Event> result = await client.GetEventsByType(eventType);

                result.Should().OnlyContain(e => e.EventType == eventType);
            });
        }
    }
}
