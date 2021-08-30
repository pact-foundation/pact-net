using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consumer.Models;

namespace Consumer
{
    public class EventsWorker
    {
        private readonly IEventProcessor _eventHandler;

        public EventsWorker(IEventProcessor eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public async Task ProcessMessages(List<Event> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }

            var operationSuccessful = true;
            foreach (var @event in events)
            {
                operationSuccessful = await _eventHandler.ProcessEvent(@event);
            }

            if (!operationSuccessful)
            {
                throw new ProcessEventException();
            }
        }
    }
}
