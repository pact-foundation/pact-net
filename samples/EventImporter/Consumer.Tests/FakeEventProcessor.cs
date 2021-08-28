using System.Threading.Tasks;
using Consumer.Models;

namespace Consumer.Tests
{
    public class FakeEventProcessor : IEventProcessor
    {
        public Task<bool> ProcessEvent(Event @event)
        {
            return Task.FromResult(true);
        }
    }
}
