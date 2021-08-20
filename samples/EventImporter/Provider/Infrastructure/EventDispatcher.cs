
using Provider.Domain;
using Provider.Domain.Models;

namespace Provider.Infrastructure
{
    public class EventDispatcher : IEventDispatcher
    {
        public void Send(Event eventSingle)
        {
            //ConvertToDto eventually...

            //send to queue here...
        }
    }
}
