using System;

namespace Provider
{
    /// <summary>
    /// A stock ticker event
    /// </summary>
    public class StockEvent
    {
        /// <summary>
        /// Stock name
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Stock price
        /// </summary>
        public decimal Price { get; init; }

        /// <summary>
        /// Event timestamp
        /// </summary>
        public DateTimeOffset Timestamp { get; init; }
    }
}
