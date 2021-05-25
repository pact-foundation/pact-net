using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Consumer.Models;
using FluentAssertions;
using FluentAssertions.Extensions;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Matchers;
using Provider.Api.Web.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Consumer.Tests
{
    public class EventsApiConsumerTests : IClassFixture<ConsumerEventApiPact>
    {
        private const string Token = "abcdef1234567890";

        private readonly ITestOutputHelper output;
        private readonly IInteractionBuilder interactions;
        private readonly EventsApiClient unauthorisedClient;
        private readonly EventsApiClient authorisedClient;

        public EventsApiConsumerTests(ConsumerEventApiPact data, ITestOutputHelper output)
        {
            data.Config.Outputters = new List<IOutput>
            {
                new ConsoleOutput(),
                new XUnitOutput(output)
            };

            this.output = output;
            this.interactions = data.Interactions;

            this.unauthorisedClient = new EventsApiClient(data.Interactions.MockProviderUri.AbsoluteUri);
            this.authorisedClient = new EventsApiClient(data.Interactions.MockProviderUri.AbsoluteUri, Token);
        }

        [Fact]
        public async Task GetAllEvents_WithNoAuthorizationToken_ShouldFail()
        {
            this.interactions
                .UponReceiving("a request to retrieve all events with no authorization")
                    .Given("there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'")
                    .WithRequest(HttpMethod.Get, "/events")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(HttpStatusCode.Unauthorized)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new
                    {
                        message = "Authorization has been denied for this request."
                    });

            await this.unauthorisedClient.Invoking(c => c.GetAllEvents()).Should().ThrowAsync<Exception>();
            this.interactions.Verify();
        }

        [Fact]
        public async Task GetAllEvents_WhenCalled_ReturnsAllEvents()
        {
            var expected = new[]
            {
                new
                {
                    eventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                    timestamp = "2014-06-30T01:37:41.0660548",
                    eventType = "SearchView"
                },
                new
                {
                    eventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    timestamp = "2014-06-30T01:37:52.2618864",
                    eventType = "DetailsView"
                },
                new
                {
                    eventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                    timestamp = "2014-06-30T01:38:00.8518952",
                    eventType = "SearchView"
                }
            };

            this.interactions
                .UponReceiving("a request to retrieve all events")
                    .Given("there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'")
                    .WithRequest(HttpMethod.Get, "/events")
                    .WithHeader("Accept", "application/json")
                    .WithHeader("Authorization", $"Bearer {Token}")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(expected);

            //Act
            IEnumerable<Event> events = await this.authorisedClient.GetAllEvents();

            //Assert
            events.Should().BeEquivalentTo(expected);
            this.interactions.Verify();
        }

        [Fact]
        public async Task CreateEvent_WhenCalledWithEvent_Succeeds()
        {
            //Arrange
            var eventId = Guid.Parse("1F587704-2DCC-4313-A233-7B62B4B469DB");
            var dateTime = 1.July(2011).At(1, 41, 3);
            DateTimeFactory.Now = () => dateTime;

            this.interactions
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

            //Act / Assert
            await this.authorisedClient.CreateEvent(eventId);
            this.interactions.Verify();
        }

        [Fact]
        public async Task IsAlive_WhenApiIsAlive_ReturnsTrue()
        {
            //Arrange
            this.interactions
                .UponReceiving("a request to check the api status")
                    .WithRequest(HttpMethod.Get, "/stats/status")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new
                    {
                        alive = true,
                        _links = new
                        {
                            uptime = new
                            {
                                href = "/stats/uptime"
                            }
                        }
                    });

            //Act
            var result = await this.unauthorisedClient.IsAlive();

            //Assert
            result.Should().BeTrue();
            this.interactions.Verify();
        }

        [Fact]
        public async Task UpSince_WhenApiIsAliveAndWeRetrieveUptime_ReturnsUpSinceDate()
        {
            //Arrange
            var upSinceDate = 27.June(2014).At(23, 51, 12).AsUtc();

            this.interactions
                .UponReceiving("a request to check the api status")
                    .WithRequest(HttpMethod.Get, "/stats/status")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new
                    {
                        alive = true,
                        _links = new
                        {
                            uptime = new
                            {
                                href = "/stats/uptime"
                            }
                        }
                    });

            this.interactions
                .UponReceiving("a request to check the api uptime")
                    .WithRequest(HttpMethod.Get, "/stats/uptime")
                    .WithHeader("Accept", "application/json")
                .WillRespond()
                .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new
                    {
                        upSince = upSinceDate
                    });

            //Act
            var result = await this.unauthorisedClient.UpSince();

            //Assert
            result.Should().Be(upSinceDate);
            this.interactions.Verify();
        }

        [Fact]
        public async Task GetEventById_WhenTheEventExists_ReturnsEvent()
        {
            //Arrange
            var expected = new Event
            {
                EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                EventType = "DetailsView",
                Timestamp = DateTime.UtcNow
            };

            this.interactions
                .UponReceiving($"a request to retrieve event with id '{expected.EventId}'")
                    .Given($"there is an event with id '{expected.EventId}'")
                    .WithRequest(HttpMethod.Get, $"/events/{expected.EventId}")
                    .WithHeader("Accept", "application/json")
                    .WithHeader("Authorization", $"Bearer {Token}")
                .WillRespond()
                    .WithStatus(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithHeader("Server", "RubyServer")
                .WithJsonBody(new
                {
                    eventId = Match.Regex(expected.EventId.ToString(), "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$"),
                    eventType = Match.Type(expected.EventType),
                    timestamp = Match.Regex(expected.Timestamp.ToString("o"), "^(-?(?:[1-9][0-9]*)?[0-9]{4})-(1[0-2]|0[1-9])-(3[0-1]|0[1-9]|[1-2][0-9])T(2[0-3]|[0-1][0-9]):([0-5][0-9]):([0-5][0-9])(\\.[0-9]+)?(Z|[+-](?:2[0-3]|[0-1][0-9]):[0-5][0-9])?$")
                });

            //Act
            var result = await this.authorisedClient.GetEventById(expected.EventId);

            //Assert
            result.Should().BeEquivalentTo(expected);
            this.interactions.Verify();
        }

        [Fact]
        public async Task GetEventsByType_WhenOneEventWithTheTypeExists_ReturnsEvent()
        {
            //Arrange
            const string eventType = "DetailsView";

            this.interactions
                .UponReceiving($"a request to retrieve events with type '{eventType}'")
                    .Given($"there is one event with type '{eventType}'")
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

            //Act
            var result = await this.authorisedClient.GetEventsByType(eventType);

            //Assert
            result.Should().OnlyContain(e => e.EventType == eventType);
            this.interactions.Verify();
        }
    }
}