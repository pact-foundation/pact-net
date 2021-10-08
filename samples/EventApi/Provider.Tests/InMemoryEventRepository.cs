using System.Collections.Generic;
using System.Linq;
using Provider.Api.Web.Models;
using Provider.Controllers;

namespace Provider.Tests
{
    public class InMemoryEventRepository : IEventRepository
    {
        public IList<Event> Events { get; } = new List<Event>();

        public IList<Event> GetAllEvents()
        {
            return Events;
        }

        internal void AddEvents(List<Event> eventsToAdd)
        {
            foreach (var @event in eventsToAdd)
            {
                AddEvent(@event);
            }
        }

        private void AddEvent(Event @event)
        {
            if (Events.Any(e => e.EventId == @event.EventId))
            {
                return;
            }

            Events.Add(@event);
        }
    }
}
