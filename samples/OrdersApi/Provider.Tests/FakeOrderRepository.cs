using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Provider.Orders;

namespace Provider.Tests
{
    /// <summary>
    /// Fake for <see cref="IOrderRepository"/>
    /// </summary>
    public class FakeOrderRepository : IOrderRepository
    {
        private readonly ConcurrentDictionary<int, OrderDto> orders = new();

        /// <summary>
        /// Get an order by ID
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order</returns>
        /// <exception cref="KeyNotFoundException">Order with the given ID was not found</exception>
        public Task<OrderDto> GetAsync(int id)
        {
            OrderDto order = this.orders[id];
            return Task.FromResult(order);
        }

        /// <summary>
        /// Insert an order
        /// </summary>
        /// <param name="order">Order to insert</param>
        /// <returns>Awaitable</returns>
        public Task InsertAsync(OrderDto order)
        {
            this.orders[order.Id] = order;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Update an order
        /// </summary>
        /// <param name="order">Order with updated state</param>
        /// <returns>Awaitable</returns>
        public Task UpdateAsync(OrderDto order)
        {
            this.orders[order.Id] = order;
            return Task.CompletedTask;
        }
    }
}
