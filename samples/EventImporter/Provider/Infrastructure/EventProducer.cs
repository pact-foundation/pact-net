using System.Collections.Generic;
using Provider.Domain;
using Provider.Domain.Models;

namespace Provider.Infrastructure
{
    public class EventProducer : IEventProducer
    {
        public void SendAsync(IReadOnlyCollection<Event> events)
        {
            //send to queue here...
        }
    }
}
