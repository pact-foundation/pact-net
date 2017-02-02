using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consumer.Tests.Models;
using PactNet;
using Xunit;
using PactNet.Models.Messaging;
using PactNet.Models.Messaging.Consumer.Dsl;

namespace Consumer.Tests
{
    public class RecieverPactTest
    {
        [Fact]
        public void ProducePact()
        {

            MessagedEvent testEvent = new MessagedEvent()
            {
                EventId = Guid.NewGuid(),
                EventType = "Parking Lot Party",
                Timestamp = DateTime.UtcNow,
                Location = new Location()
                {
                    Latitude = new Coordinate() { Degrees = 41, Minutes = 0, Seconds = 47.0 },
                    Longitude = new Coordinate() { Degrees = 24, Minutes = 17, Seconds = 11.1}
                }
            };

            var body = new PactDslJsonRoot()
                    .StringType("eventType", testEvent.EventType)
                    .GuidMatcher("eventId", testEvent.EventId)
                    .DateFormat("timestamp", "O", testEvent.Timestamp)
                    .Object("location")
                        .Object("latitude")
                            .IntegerMatcher("degrees", testEvent.Location.Latitude.Degrees)
                            .IntegerMatcher("minutes", testEvent.Location.Latitude.Minutes)
                            .DecimalMatcher("seconds", testEvent.Location.Latitude.Seconds)
                        .CloseObject()
                        .Object("longitude")
                            .IntegerMatcher("degrees", testEvent.Location.Longitude.Degrees)
                            .IntegerMatcher("minutes", testEvent.Location.Longitude.Minutes)
                            .DecimalMatcher("seconds", testEvent.Location.Longitude.Seconds)
                        .CloseObject()
                    .CloseObject();       

            IPactMessagingBuilder builder = new PactMessageBuilder();

            builder.ServiceConsumer("Consumer-dotNet")
                .HasPactWith("Provider.Messaging-dotNet");

            builder
            .WithContent(new Message()
                .Given("A new party event")
                .ExpectsToRecieve("event.party")
                .WithBody(body))
            .Build();
        }
    }
}
