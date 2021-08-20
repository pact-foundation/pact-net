using System.Collections.Generic;

using Provider.Domain.Models;

namespace Provider.Domain.Handlers
{
    public interface IEventHandler
    {
        void ImportAll(IReadOnlyCollection<Event> events);
        void DispatchLast(IReadOnlyCollection<Event> events);
    }
}
