using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PactNet.AspNetCore.ProviderState;
using PactNet.Infrastructure.Outputters;
using PactNet.Native.Verifier;
using PactNet.Verifier;
using Provider.Api.Web.Models;
using Provider.Api.Web.Tests;
using Provider.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class EventApiTests : IClassFixture<EventApiFixture>
    {
        private readonly EventApiFixture fixture;
        private readonly ITestOutputHelper output;
        private readonly ProviderStateOptions options;
        private InMemoryEventRepository repository;

        public EventApiTests(EventApiFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
            this.options = fixture.GetOptions();
            this.repository = fixture.GetFakeEventRepository() as InMemoryEventRepository;
        }

        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(this.output)
                }
            };

            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "Consumer.Tests",
                                           "pacts",
                                           "Event API Consumer-Event API.json");

            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .ServiceProvider("Event API", this.fixture.ServerUri)
                .HonoursPactWith("Event API Consumer")
                .FromPactFile(new FileInfo(pactPath))
                .WithProviderStateUrl(new Uri(this.fixture.ServerUri, options.RouteProviderState))
                .WithProviderStates(providerStates =>
                {
                    providerStates
                        .Add(
                            "there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'",
                            this.InsertEventsIntoDatabase, StateAction.Setup)
                        .Add("there is an event with id '83f9262f-28f1-4703-ab1a-8cfd9e8249c9'", InsertEventIntoDatabase)
                        .Add("there is one event with type 'DetailsView'", this.EnsureOneDetailsViewEventExists)
                        .Add("at least one event of type eventType exists", this.EnsureOneEventWithTypeExists)
                        .Add("at least one event of type eventType exists", this.ClearEventsWithType, StateAction.Teardown);
                })
                .Verify();
            //Assert.Empty(this.repository.Events);
        }

        private void InsertEventsIntoDatabase()
        {
            var eventsToAdd = new List<Event>
            {
                new Event
                {
                    EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:41.0660548"),
                    EventType = "SearchView"
                },
                new Event
                {
                    EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:52.2618864"),
                    EventType = "DetailsView"
                },
                new Event
                {
                    EventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                    Timestamp = DateTime.Parse("2014-06-30T01:38:00.8518952"),
                    EventType = "SearchView"
                }
            };

            this.repository.AddEvents(eventsToAdd);
        }

        private void InsertEventIntoDatabase()
        {
            var eventsToAdd = new List<Event>
            {
                new Event
                {
                    EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:52.2618864"),
                    EventType = "DetailsView"
                }
            };

            this.repository.AddEvents(eventsToAdd);
        }

        private void EnsureOneDetailsViewEventExists()
        {
            var eventsToAdd = new List<Event>
            {
                new Event
                {
                    EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:52.2618864"),
                    EventType = "DetailsView"
                }
            };

            this.repository.AddEvents(eventsToAdd);
        }

        private void EnsureOneEventWithTypeExists(IDictionary<string, string> args)
        {
            var eventType = args["eventType"];

            var eventsToAdd = new List<Event>
            {
                new Event
                {
                    EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:52.2618864"),
                    EventType = eventType
                }
            };

            this.repository.AddEvents(eventsToAdd);
        }

        private void ClearEventsWithType(IDictionary<string, string> args)
        {
            var eventType = args["eventType"];

            this.repository.RemoveAllEventsByType(eventType);
        }
    }
}
