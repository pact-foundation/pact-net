using System;
using System.Collections.Generic;
using System.Linq;
using PactNet;
using PactNet.Consumer;
using PactNet.Consumer.Mocks;
using Xunit;

namespace Consumer.Tests
{
    public class EventsApiConsumerTests : IDisposable
    {
        private const int MockServerPort = 1234;
        private readonly string _mockServerBaseUri = String.Format("http://localhost:{0}", MockServerPort);

        private readonly IPactConsumer _pact;
        private readonly IMockProvider _mockProvider;

        public EventsApiConsumerTests()
        {
            _pact = new Pact().ServiceConsumer("Consumer")
                .HasPactWith("Event API");

            _mockProvider = _pact.MockService(MockServerPort);
        }

        public void Dispose()
        {
            _pact.Dispose();
        }

        [Fact]
        public void GetAllEvents_WhenCalled_ReturnsAllEvents()
        {
            _mockProvider.UponReceiving("A GET request to retrieve all events")
                .With(new PactProviderRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/events",
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" }
                    }
                })
                .WillRespondWith(new PactProviderResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new List<dynamic>
                    {
                        new 
                        {
                            EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                            Timestamp = "2014-06-30T01:37:41.0660548Z",
                            EventType = "JobSearchView"
                        },
                        new
                        {
                            EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                            Timestamp = "2014-06-30T01:37:52.2618864Z",
                            EventType = "JobDetailsView"
                        },
                        new
                        {
                            EventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                            Timestamp = "2014-06-30T01:38:00.8518952Z",
                            EventType = "JobSearchView"
                        }
                    }
                });

            var consumer = new EventsApiClient(_mockServerBaseUri);

            //Act
            var events = consumer.GetAllEvents();

            Assert.NotEmpty(events);
            Assert.Equal(3, events.Count());
        }

        [Fact]
        public void CreateEvent_WhenCalledWithEvent_Suceeds()
        {
            var eventId = Guid.Parse("1F587704-2DCC-4313-A233-7B62B4B469DB");
            var dateTime = new DateTime(2011, 07, 01, 01, 41, 03);
            DateTimeFactory.Now = () => dateTime;

            _mockProvider.UponReceiving("A POST request to create a new event")
                .With(new PactProviderRequest
                {
                    Method = HttpVerb.Post,
                    Path = "/events",
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new
                    {
                        EventId = eventId,
                        Timestamp = dateTime.ToString("O"),
                        EventType = "JobDetailsView"
                    }
                })
                .WillRespondWith(new PactProviderResponse
                {
                    Status = 201
                });

            var consumer = new EventsApiClient(_mockServerBaseUri);

            //Act
            consumer.CreateEvent(eventId);
        }
    }
}
