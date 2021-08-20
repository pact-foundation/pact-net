using System.Collections.Generic;
using System.Linq;

using Provider.Domain.Models;

namespace Provider.Domain.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IEventProducer _eventProducer;
        private readonly IEventDispatcher _eventDispatcher;

        public EventHandler(IEventProducer eventProducer, IEventDispatcher eventDispatcher)
        {
            _eventProducer = eventProducer;
            _eventDispatcher = eventDispatcher;
        }

        public void ImportAll(IReadOnlyCollection<Event> events)
        {
            _eventProducer.Send(events);
        }

        public void DispatchLast(IReadOnlyCollection<Event> events)
        {
            if (events is null)
            {
                return;
            }

            _eventDispatcher.Send(events.OrderByDescending(x => x.Timestamp).FirstOrDefault());
        }
    }
}
