using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using PactNet.AspNetCore.Messaging;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using PactNet.Verifier.Messaging;
using Provider.Api;
using Provider.Api.Controllers;
using Provider.Domain;
using Provider.Domain.Models;
using Provider.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using EventHandler = Provider.Domain.Handlers.EventHandler;

namespace Provider.Tests
{
    public class EventImporterProviderTests : IClassFixture<EventImporterFixture>
    {
        private readonly EventImporterFixture fixture;
        private readonly ITestOutputHelper output;
        private readonly MessagingVerifierOptions options;

        public EventImporterProviderTests(EventImporterFixture fixture, ITestOutputHelper output)
        {
            this.fixture = fixture;
            this.output = output;
            this.options = fixture.GetOptions();
        }

        [Fact]
        public void EnsureEventApiHonoursPactWithConsumer()
        {
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(output)
                }
            };

            //C
            string pactPath = Path.Combine("..",
                                           "..",
                                           "..",
                                           "..",
                                           "Consumer.Tests",
                                           "pacts",
                                           "Event API Consumer V3 Message-Event API V3 Message.json");

            //Act / Assert
            IPactVerifier pactVerifier = new PactVerifier(config);
            pactVerifier
                .MessagingProvider("Event API V3 Message", this.fixture.ServerUri, options.BasePath)
                .WithProviderMessages(scenarios =>
                {
                    scenarios.Add(this.GetEventImporterScenarios)
                             .Add("a single event", () => new Event("single"))
                             .Add("a single event with metadata", new { foo = "bar" }, () => new Event("event with metadata"));
                })
                .HonoursPactWith("Event API Consumer V3 Message")
                .FromPactFile(new FileInfo(pactPath))
                .Verify();
        }

        private Scenario GetEventImporterScenarios
        {
            get
            {
                //-----------------------------------------------------
                // A complex example of scenario setting.
                //
                // - here we chose to generate the object from the business component
                //   by mocking the last call before sending information to the queue
                //-----------------------------------------------------
                return MessageScenarioBuilder
                      .WhenReceiving("receiving events from the queue")
                      .WithMetadata(new { key = "valueKey" })
                      .WithContent(() =>
                       {
                           //The component that will send the message to the queue (Kafka, Rabbit, etc)
                           var mockEventProducer = new Mock<IEventProducer>();

                           List<Event> eventsPushed = null;

                           //The last method called before sending the message to the queue needs to be mocked
                           //... and the message (here eventPushed) needs to be intercepted in the test for verification
                           mockEventProducer
                              .Setup(x => x.Send(It.IsAny<IReadOnlyCollection<Event>>()))
                              .Callback((IReadOnlyCollection<Event> events) => { eventsPushed = events.ToList(); });

                           //We call the endpoint with the payload to generate a real-life message
                           //The fake event repository would represent an actual storage system
                           var controller = new EventsController(new FakeEventRepository(),
                                                                 new EventHandler(mockEventProducer.Object, new EventDispatcher()));

                           controller.ImportToQueue();

                           return eventsPushed;
                       });

                //-----------------------------------------------------
                // A simple example of scenario setting.
                //
                // - here we chose to generate the object manually
                //
                //   WARNING: be careful with this approach, you never
                //   guarantee your actual code is in sync with the
                //   manually generated object below.
                //-----------------------------------------------------
                //return
                //    MessageScenarioBuilder
                //        .WhenReceiving("receiving events from the queue")
                //        .WithMetadata(new { key = "valueKey" })
                //        .WithContent(new List<Event>
                //        {
                //            new Event
                //            {
                //                EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                //                EventType = "SearchView",
                //                Timestamp = DateTime.Parse("2014-06-30T01:37:41.0660548")
                //            }
                //        });
            }
        }

        private class FakeEventRepository : IEventRepository
        {
            public IReadOnlyCollection<Event> GetAllEvents()
            {
                return new List<Event>
                {
                    new Event
                    {
                        EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                        EventType = "SearchView",
                        Timestamp = DateTime.Parse("2014-06-30T01:37:41.0660548")
                    }
                };
            }
        }
    }
}
