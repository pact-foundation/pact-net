using System.Threading.Tasks;
using Consumer.Models;

namespace Consumer
{
    public interface IEventProcessor
    {
        Task<bool> ProcessEvent(Event @event);
    }
}
