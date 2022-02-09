
using Provider.Domain.Models;

namespace Provider.Domain
{
    public interface IEventDispatcher
    {
        void Send(Event eventSingle);
    }
}
