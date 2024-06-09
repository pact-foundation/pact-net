using System.Threading.Tasks;

namespace Consumer
{
    public interface IOrdersClient
    {
        /// <summary>
        /// Get an order by ID
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Order</returns>
        Task<OrderDto> GetOrderAsync(int orderId);

        /// <summary>
        /// Update the status of an order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <param name="status">New status</param>
        /// <returns>Awaitable</returns>
        ValueTask UpdateOrderAsync(int orderId, OrderStatus status);
    }
}
