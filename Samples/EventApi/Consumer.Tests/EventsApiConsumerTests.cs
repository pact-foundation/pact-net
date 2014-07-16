using System;
using System.Collections.Generic;
using System.Linq;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace Consumer.Tests
{
    public class EventsApiConsumerTests : IUseFixture<ConsumerEventApiPact>
    {
        private ConsumerEventApiPact _data;
            
        public void SetFixture(ConsumerEventApiPact data)
        {
            _data = data;
        }
        
        [Fact]
        public void GetAllEvents_WhenCalled_ReturnsAllEvents()
        {
            //Arrange
            _data.MockProviderService.Given("There are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'")
                .UponReceiving("A GET request to retrieve all events")
                .With(new PactProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/events",
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" }
                    }
                })
                .WillRespondWith(new PactProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new []
                    {
                        new 
                        {
                            eventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                            timestamp = "2014-06-30T01:37:41.0660548Z",
                            eventType = "JobSearchView"
                        },
                        new
                        {
                            eventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                            timestamp = "2014-06-30T01:37:52.2618864Z",
                            eventType = "JobDetailsView"
                        },
                        new
                        {
                            eventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                            timestamp = "2014-06-30T01:38:00.8518952Z",
                            eventType = "JobSearchView"
                        }
                    }
                })
                .RegisterInteraction();

            var consumer = new EventsApiClient(_data.MockServerBaseUri);

            //Act
            var events = consumer.GetAllEvents();

            //Assert
            Assert.NotEmpty(events);
            Assert.Equal(3, events.Count());
        }

        [Fact]
        public void CreateEvent_WhenCalledWithEvent_Suceeds()
        {
            //Arrange
            var eventId = Guid.Parse("1F587704-2DCC-4313-A233-7B62B4B469DB");
            var dateTime = new DateTime(2011, 07, 01, 01, 41, 03);
            DateTimeFactory.Now = () => dateTime;

            _data.MockProviderService.UponReceiving("A POST request to create a new event")
                .With(new PactProviderServiceRequest
                {
                    Method = HttpVerb.Post,
                    Path = "/events",
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new
                    {
                        eventId,
                        timestamp = dateTime.ToString("O"),
                        eventType = "JobDetailsView"
                    }
                })
                .WillRespondWith(new PactProviderServiceResponse
                {
                    Status = 201
                })
                .RegisterInteraction();

            var consumer = new EventsApiClient(_data.MockServerBaseUri);

            //Act / Assert
            consumer.CreateEvent(eventId);
        }

        [Fact]
        public void IsAlive_WhenApiIsAlive_Suceeds()
        {
            //Arrange
            _data.MockProviderService.UponReceiving("A GET request to check the api status")
                .With(new PactProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/stats/status"
                })
                .WillRespondWith(new PactProviderServiceResponse
                {
                    Status = 200,
                    Body = "alive"
                })
                .RegisterInteraction();

            var consumer = new EventsApiClient(_data.MockServerBaseUri);

            //Act
            var result = consumer.IsAlive();

            //Assert
            Assert.Equal(true, result);
        }
    }
}
