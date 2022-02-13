using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provider
{
    /// <summary>
    /// Event sender
    /// </summary>
    public class StockEventSender : IStockEventSender
    {
        /// <summary>
        /// Send the events
        /// </summary>
        /// <param name="events">Events to send</param>
        /// <returns>Awaitable</returns>
        public ValueTask SendAsync(ICollection<StockEvent> events)
        {
            // actually send the events...

            return ValueTask.CompletedTask;
        }
    }
}
