using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Provider.PubSub;

namespace Provider.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMessagePublisher publisher;

        // NOTE: for this demo this uses an in-memory store but in reality this would store to a database or something
        private readonly ConcurrentDictionary<int, OrderDto> orders = new();

        /// <summary>
        /// Initialises a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="publisher">Message publisher</param>
        public OrderRepository(IMessagePublisher publisher)
        {
            this.publisher = publisher;
        }

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
        public async Task InsertAsync(OrderDto order)
        {
            this.orders[order.Id] = order;

            // notify subscribers of the new order
            await this.publisher.PublishAsync(new OrderCreatedEvent(order.Id));
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
