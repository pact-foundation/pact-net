using System.Collections.Generic;
using Provider.Api.Web.Models;

namespace Provider.Api.Web.Publishers
{
    public class EventsPublisher
    {
        public void PublishEventUpdated(Event myEvent)
        {
            var eventUpdatedMessage = GetEventUpdatedMessage(myEvent);

            //Publish event
        }

        public Message<Event> GetEventUpdatedMessage(Event myUpdatedEvent)
        {
            return new Message<Event>
            {
                Contents = myUpdatedEvent,
                Metadata = new Dictionary<string, string>
                {
                    {"ContentType", "application:json;"}
                }
            };
        }

    }
}