using System.Threading.Tasks;

namespace Consumer
{
    /// <summary>
    /// Service for fulfilling orders
    /// </summary>
    public class FulfilmentService : IFulfilmentService
    {
        private readonly IOrdersClient client;

        /// <summary>
        /// Initialises a new instance of the <see cref="FulfilmentService"/> class.
        /// </summary>
        /// <param name="client">Orders client</param>
        public FulfilmentService(IOrdersClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Fulfil the given order
        /// </summary>
        /// <param name="orderId">Order ID</param>
        /// <returns>Awaitable</returns>
        public async ValueTask FulfilOrderAsync(int orderId)
        {
            var order = await this.client.GetOrderAsync(orderId);
            await this.client.UpdateOrderAsync(order.Id, OrderStatus.Fulfilling);

            // fulfil and ship the order...

            await this.client.UpdateOrderAsync(order.Id, OrderStatus.Shipped);
        }
    }
}
