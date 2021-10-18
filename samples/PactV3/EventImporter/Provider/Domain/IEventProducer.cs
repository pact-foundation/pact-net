using System.Collections.Generic;

using Provider.Domain.Models;

namespace Provider.Domain
{
    public interface IEventProducer
    {
        void Send(IReadOnlyCollection<Event> events);
    }
}
