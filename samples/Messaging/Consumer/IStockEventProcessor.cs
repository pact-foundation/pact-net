using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consumer
{
    /// <summary>
    /// Stock event processor
    /// </summary>
    public interface IStockEventProcessor
    {
        /// <summary>
        /// Process events
        /// </summary>
        /// <param name="events">Stock events</param>
        /// <returns>Handled successfully</returns>
        Task<bool> ProcessEvents(ICollection<StockEvent> events);
    }
}
