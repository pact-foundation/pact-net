using Newtonsoft.Json;
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

        public string GetEventUpdatedMessage(Event myUpdatedEvent)
        {
            return JsonConvert.SerializeObject(myUpdatedEvent);
        }

    }
}