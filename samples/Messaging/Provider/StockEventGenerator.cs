using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;

namespace Provider
{
    /// <summary>
    /// Stock event generator
    /// </summary>
    public class StockEventGenerator : IStockEventGenerator
    {
        private readonly IStockEventSender sender;

        /// <summary>
        /// Initialises a new instance of the <see cref="StockEventSender"/> class.
        /// </summary>
        /// <param name="sender">Stock event sender</param>
        public StockEventGenerator(IStockEventSender sender)
        {
            this.sender = sender;
        }

        /// <summary>
        /// Generate new stock events
        /// </summary>
        /// <returns>Awaitable</returns>
        public async ValueTask GenerateEventsAsync()
        {
            // source some new data from somewhere - e.g. an external system or database
            // and publish them

            ICollection<StockEvent> events = new Fixture().CreateMany<StockEvent>().ToArray();

            await this.sender.SendAsync(events);
        }
    }
}
