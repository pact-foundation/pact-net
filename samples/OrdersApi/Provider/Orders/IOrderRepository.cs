using System.Collections.Generic;
using System.Threading.Tasks;

namespace Provider.Orders
{
    /// <summary>
    /// Repository for managing order instances
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Get an order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order</returns>
        /// <exception cref="KeyNotFoundException">Order with the given ID was not found</exception>
        Task<OrderDto> GetAsync(int id);

        /// <summary>
        /// Insert an order
        /// </summary>
        /// <param name="order">Order to insert</param>
        /// <returns>Awaitable</returns>
        Task InsertAsync(OrderDto order);

        /// <summary>
        /// Update an order
        /// </summary>
        /// <param name="order">Order with updated state</param>
        /// <returns>Awaitable</returns>
        Task UpdateAsync(OrderDto order);
    }
}
