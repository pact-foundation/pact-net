using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace Consumer.Tests
{
    public class EventsApiConsumerTests : IUseFixture<ConsumerEventApiPact>
    {
        private IMockProviderService _mockProviderService;
        private string _mockProviderServiceBaseUri;
            
        public void SetFixture(ConsumerEventApiPact data)
        {
            _mockProviderService = data.MockProviderService;
            _mockProviderServiceBaseUri = data.MockProviderServiceBaseUri;
            _mockProviderService.ClearInteractions();
        }

        [Fact]
        public void GetAllEvents_WithNoAuthorizationToken_ShouldFail()
        {
            //Arrange
            _mockProviderService.Given("there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'")
                .UponReceiving("a request to retrieve all events with no authorization")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/events",
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" },
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 401,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new 
                    {
                        message = "Authorization has been denied for this request."
                    }
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri);

            //Act //Assert
            Assert.Throws<HttpRequestException>(() => consumer.GetAllEvents());
            
            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void GetAllEvents_WhenCalled_ReturnsAllEvents()
        {
            //Arrange
            var testAuthToken = "SomeValidAuthToken";

            _mockProviderService.Given("there are events with ids '45D80D13-D5A2-48D7-8353-CBB4C0EAABF5', '83F9262F-28F1-4703-AB1A-8CFD9E8249C9' and '3E83A96B-2A0C-49B1-9959-26DF23F83AEB'")
                .UponReceiving("a request to retrieve all events")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/events",
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" },
                        { "Authorization", $"Bearer {testAuthToken}" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
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
                    }
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri, testAuthToken);

            //Act
            var events = consumer.GetAllEvents();

            //Assert
            Assert.NotEmpty(events);
            Assert.Equal(3, events.Count());


            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void CreateEvent_WhenCalledWithEvent_Succeeds()
        {
            //Arrange
            var eventId = Guid.Parse("1F587704-2DCC-4313-A233-7B62B4B469DB");
            var dateTime = new DateTime(2011, 07, 01, 01, 41, 03);
            DateTimeFactory.Now = () => dateTime;

            _mockProviderService.UponReceiving("a request to create a new event")
                .With(new ProviderServiceRequest
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
                        eventType = "DetailsView"
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 201
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri);

            //Act / Assert
            consumer.CreateEvent(eventId);

            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void IsAlive_WhenApiIsAlive_ReturnsTrue()
        {
            //Arrange
            _mockProviderService.UponReceiving("a request to check the api status")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Headers = new Dictionary<string, string> { { "Accept", "application/json" } },
                    Path = "/stats/status"
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
                    Body = new
                    {
                        alive = true,
                        _links = new
                        {
                            uptime = new
                            {
                                href = "/stats/uptime"
                            }
                        }
                    }
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri);

            //Act
            var result = consumer.IsAlive();

            //Assert
            Assert.Equal(true, result);

            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void UpSince_WhenApiIsAliveAndWeRetrieveUptime_ReturnsUpSinceDate()
        {
            //Arrange
            var upSinceDate = new DateTime(2014, 6, 27, 23, 51, 12, DateTimeKind.Utc);

            _mockProviderService.UponReceiving("a request to check the api status")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Headers = new Dictionary<string, string> { { "Accept", "application/json" } },
                    Path = "/stats/status"
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
                    Body = new
                    {
                        alive = true,
                        _links = new
                        {
                            uptime = new
                            {
                                href = "/stats/uptime"
                            }
                        }
                    }
                });

            _mockProviderService
                .UponReceiving("a request to check the api uptime")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Headers = new Dictionary<string, string> { { "Accept", "application/json" } },
                    Path = "/stats/uptime"
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string> { { "Content-Type", "application/json; charset=utf-8" } },
                    Body = new
                    {
                        upSince = upSinceDate
                    }
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri);

            //Act
            var result = consumer.UpSince();

            //Assert
            Assert.Equal(upSinceDate.ToString("O"), result.Value.ToString("O"));

            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void GetEventById_WhenTheEventExists_ReturnsEvent()
        {
            //Arrange
            var eventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9");
            _mockProviderService.Given(String.Format("there is an event with id '{0}'", eventId))
                .UponReceiving(String.Format("a request to retrieve event with id '{0}'", eventId))
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/events/" + eventId,
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new
                    {
                        eventId = eventId
                    }
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri);

            //Act
            var result = consumer.GetEventById(eventId);

            //Assert
            Assert.Equal(eventId, result.EventId);

            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void GetEventsByType_WhenOneEventWithTheTypeExists_ReturnsEvent()
        {
            //Arrange
            const string eventType = "DetailsView";
            _mockProviderService.Given(String.Format("there is one event with type '{0}'", eventType))
                .UponReceiving(String.Format("a request to retrieve events with type '{0}'", eventType))
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/events",
                    Query = "type=" + eventType,
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" }
                    },
                    Body = null
                })
                .WillRespondWith(new ProviderServiceResponse
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
                            eventType = eventType
                        }
                    }
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri);

            //Act
            var result = consumer.GetEventsByType(eventType);

            //Assert
            Assert.Equal(eventType, result.First().EventType);

            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void CreateBlob_WhenCalledWithBlob_Succeeds()
        {
            //Arrange
            var blobId = Guid.Parse("38C3976B-5AE8-4F2F-A8EC-46F6AEE826E2");
            var bytes = Encoding.UTF8.GetBytes("This is a test");

            _mockProviderService.UponReceiving("a request to create a new blob")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Post,
                    Path = String.Format("/blobs/{0}", blobId),
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/octet-stream" }
                    },
                    Body = bytes
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 201
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri);

            //Act / Assert
            consumer.CreateBlob(blobId, bytes, "test.txt");

            _mockProviderService.VerifyInteractions();
        }

        [Fact]
        public void GetBlob_WhenCalledWithId_Succeeds()
        {
            //Arrange
            var blobId = Guid.Parse("38C3976B-5AE8-4F2F-A8EC-46F6AEE826E2");
            var bytes = Encoding.UTF8.GetBytes("This is a test");

            _mockProviderService.UponReceiving("a request to get a new blob by id")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = String.Format("/blobs/{0}", blobId)
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 201,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "text/plain" }
                    },
                    Body = "This is a test"
                });

            var consumer = new EventsApiClient(_mockProviderServiceBaseUri);

            //Act / Assert
            var content = consumer.GetBlob(blobId);

            Assert.True(bytes.SequenceEqual(content));

            _mockProviderService.VerifyInteractions();
        }
    }
}
