using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consumer
{
    /// <summary>
    /// Stock event processor
    /// </summary>
    public class StockEventProcessor : IStockEventProcessor
    {
        /// <summary>
        /// Process events
        /// </summary>
        /// <param name="events">Stock events</param>
        /// <returns>Handled successfully</returns>
        public Task<bool> ProcessEvents(ICollection<StockEvent> events)
        {
            // ...actually process the event...

            return Task.FromResult(true);
        }
    }
}
