using System.Collections.Generic;

using Provider.Domain.Models;

namespace Provider.Api
{
    public interface IEventHandler
    {
        void ImportAllEvents(IReadOnlyCollection<Event> events);
    }
}
