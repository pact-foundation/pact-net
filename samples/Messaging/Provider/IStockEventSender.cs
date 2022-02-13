using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provider
{
    /// <summary>
    /// Event sender
    /// </summary>
    public interface IStockEventSender
    {
        /// <summary>
        /// Send the events
        /// </summary>
        /// <param name="events">Events to send</param>
        /// <returns>Awaitable</returns>
        ValueTask SendAsync(ICollection<StockEvent> events);
    }
}
