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
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class EventsApiConsumerTests
    {
        private const string Token = "SomeValidAuthToken";

        private readonly IPactBuilderV2 pact;

        public EventsApiConsumerTests(ITestOutputHelper output)
        {
            var config = new PactConfig
            {
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

            IPactV2 pact = Pact.V2("Event API Consumer", "Event API", config);
            this.pact = pact.WithHttpInteractions();
        }

        [Fact]
        public async Task GetAllEvents_WithNoAuthorizationToken_ShouldFail()
        {
            this.pact
                .UponReceiving("a request to retrieve all events with no authorization")
                    .Given("there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'")
                    .WithRequest(HttpMethod.Get, "/events")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.Unauthorized);

            await this.pact.VerifyAsync(async ctx =>
            {
                var client = new EventsApiClient(ctx.MockServerUri);

                await client.Invoking(c => c.GetAllEvents()).Should().ThrowAsync<Exception>();
            });
        }

        [Fact]
        public async Task GetAllEvents_WhenCalled_ReturnsAllEvents()
        {
            var example = new Event
            {
                EventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                Timestamp = DateTime.Parse("2014-06-30T01:38:00.8518952"),
                EventType = "SearchView"
            };

            this.pact
                .UponReceiving("a request to retrieve all events")
                    .WithRequest(HttpMethod.Get, "/events")
                    .WithHeader("Accept", "application/json")
                    .WithHeader("Authorization", $"Bearer {Token}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.MinType(new
                    {
                        eventId = Match.Type("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                        timestamp = Match.Type("2014-06-30T01:38:00.8518952"),
                        eventType = Match.Regex("SearchView", "SearchView|DetailsView")
                    }, 1));

            await this.pact.VerifyAsync(async ctx =>
            {
                var client = new EventsApiClient(ctx.MockServerUri, Token);

                IEnumerable<Event> events = await client.GetAllEvents();

                events.Should().BeEquivalentTo(new[] { example });
            });
        }

        [Fact]
        public async Task GetEventsByType_WhenOneEventWithTheTypeExists_ReturnsEvent()
        {
            //Arrange
            const string eventType = "DetailsView";

            this.pact
                .UponReceiving($"a request to retrieve events with type '{eventType}'")
                    .Given($"there is one event with type '{eventType}'")
                    .WithRequest(HttpMethod.Get, "/events")
                    .WithQuery("type", eventType)
                    .WithHeader("Accept", "application/json")
                    .WithHeader("Authorization", $"Bearer {Token}")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.MinType(new
                    {
                        eventId = Match.Type("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                        timestamp = Match.Type("2014-06-30T01:37:41.0660548"),
                        eventType
                    }, 1));

            await this.pact.VerifyAsync(async ctx =>
            {
                var client = new EventsApiClient(ctx.MockServerUri, Token);

                IEnumerable<Event> result = await client.GetEventsByType(eventType);

                result.Should().OnlyContain(e => e.EventType == eventType);
            });
        }

        [Fact]
        public async Task CreateEvent_WhenCalledWithEvent_Succeeds()
        {
            var eventId = Guid.Parse("1F587704-2DCC-4313-A233-7B62B4B469DB");
            var dateTime = 1.July(2011).At(1, 41, 3);
            DateTimeFactory.Override = dateTime;

            this.pact
                .UponReceiving("a request to create a new event")
                    .WithRequest(HttpMethod.Post, "/events")
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithHeader("Authorization", $"Bearer {Token}")
                    .WithJsonBody(new
                    {
                        eventId,
                        timestamp = dateTime.ToString("O"),
                        eventType = "DetailsView"
                    })
                .WillRespond()
                    .WithStatus(HttpStatusCode.Created);

            await this.pact.VerifyAsync(async ctx =>
            {
                var client = new EventsApiClient(ctx.MockServerUri, Token);

                await client.CreateEvent(eventId);
            });
        }

        [Fact]
        public async Task IsAlive_WhenApiIsAlive_ReturnsTrue()
        {
            this.pact
                .UponReceiving("a request to check the api status")
                    .WithRequest(HttpMethod.Get, "/stats/status")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Type(new
                    {
                        alive = Match.Type(true),
                        _links = Match.Type(new
                        {
                            self = Match.Type(new
                            {
                                href = Match.Type("/stats/status")
                            }),
                            uptime = Match.Type(new
                            {
                                href = Match.Type("/stats/uptime")
                            }),
                            version = Match.Type(new
                            {
                                href = Match.Type("/stats/version")
                            }),
                        })
                    }));

            await this.pact.VerifyAsync(async ctx =>
            {
                var client = new EventsApiClient(ctx.MockServerUri);

                bool result = await client.IsAlive();

                result.Should().BeTrue();
            });
        }

        [Fact]
        public async Task UpSince_WhenApiIsAliveAndWeRetrieveUptime_ReturnsUpSinceDate()
        {
            var upSinceDate = 27.June(2014).At(23, 51, 12).AsUtc();

            this.pact
                .UponReceiving("a request to check the api status")
                    .WithRequest(HttpMethod.Get, "/stats/status")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Type(new
                    {
                        alive = Match.Type(true),
                        _links = Match.Type(new
                        {
                            self = Match.Type(new
                            {
                                href = Match.Type("/stats/status")
                            }),
                            uptime = Match.Type(new
                            {
                                href = Match.Type("/stats/uptime")
                            }),
                            version = Match.Type(new
                            {
                                href = Match.Type("/stats/version")
                            }),
                        })
                    }));

            this.pact
                .UponReceiving("a request to check the api uptime")
                    .WithRequest(HttpMethod.Get, "/stats/uptime")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(Match.Type(new
                    {
                        upSince = Match.Type(upSinceDate)
                    }));

            await this.pact.VerifyAsync(async ctx =>
            {
                var client = new EventsApiClient(ctx.MockServerUri, Token);

                DateTime? result = await client.UpSince();

                result.Should().Be(upSinceDate);
            });
        }

        [Fact]
        public async Task GetEventById_WhenTheEventExists_ReturnsEvent()
        {
            var expected = new Event
            {
                EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                EventType = "DetailsView",
                Timestamp = 27.June(2014).At(23, 51, 12).AsUtc()
            };

            this.pact
                .UponReceiving($"a request to retrieve event with id '{expected.EventId}'")
                    .Given($"there is an event with id '{expected.EventId}'")
                    .WithRequest(HttpMethod.Get, $"/events/{expected.EventId}")
                    .WithHeader("accept", "application/json")
                    .WithHeader("authorization", $"Bearer {Token}")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("content-type", "application/json; charset=utf-8")
                    .WithJsonBody(new
                    {
                        eventId = Match.Regex(expected.EventId.ToString(), "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$"),
                        eventType = Match.Regex(expected.EventType, "SearchView|DetailsView"),
                        timestamp = Match.Regex(expected.Timestamp.ToString("o"), "^(-?(?:[1-9][0-9]*)?[0-9]{4})-(1[0-2]|0[1-9])-(3[0-1]|0[1-9]|[1-2][0-9])T(2[0-3]|[0-1][0-9]):([0-5][0-9]):([0-5][0-9])(\\.[0-9]+)?(Z|[+-](?:2[0-3]|[0-1][0-9]):[0-5][0-9])?$")
                    });

            await this.pact.VerifyAsync(async ctx =>
            {
                var client = new EventsApiClient(ctx.MockServerUri, Token);

                Event result = await client.GetEventById(expected.EventId);

                result.Should().BeEquivalentTo(expected);
            });
        }
    }
}
