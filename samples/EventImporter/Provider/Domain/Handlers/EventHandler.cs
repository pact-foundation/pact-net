using System.Collections.Generic;

using Provider.Api;
using Provider.Domain.Models;

namespace Provider.Domain.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IEventProducer _eventProducer;

        public EventHandler(IEventProducer eventProducer)
        {
            _eventProducer = eventProducer;
        }

        public void ImportEvents(IReadOnlyCollection<Event> events)
        {
            _eventProducer.SendAsync(events);
        }
    }
}
