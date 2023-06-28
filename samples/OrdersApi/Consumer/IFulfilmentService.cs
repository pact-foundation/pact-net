using System.Threading.Tasks;

namespace Consumer
{
    /// <summary>
    /// Service for fulfilling orders
    /// </summary>
    public interface IFulfilmentService
    {
        /// <summary>
        /// Fulfil the given order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Awaitable</returns>
        ValueTask FulfilOrderAsync(int orderId);
    }
}
