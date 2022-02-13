using System.Threading.Tasks;

namespace Provider
{
    /// <summary>
    /// Stock event generator
    /// </summary>
    public interface IStockEventGenerator
    {
        /// <summary>
        /// Generate new stock events
        /// </summary>
        /// <returns>Awaitable</returns>
        ValueTask GenerateEventsAsync();
    }
}
