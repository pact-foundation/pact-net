using System.Threading.Tasks;

namespace Consumer
{
    /// <summary>
    /// Subscribes to events on the Orders API
    /// </summary>
    public class OrderCreatedConsumer
    {
        private readonly IFulfilmentService fulfilment;

        /// <summary>
        /// Initialises a new instance of the <see cref="OrderCreatedConsumer"/> class.
        /// </summary>
        /// <param name="fulfilment">Fulfilment service</param>
        public OrderCreatedConsumer(IFulfilmentService fulfilment)
        {
            this.fulfilment = fulfilment;
        }

        /// <summary>
        /// Process an event which identifies that an order has been created
        /// </summary>
        /// <param name="message">Order created event</param>
        /// <returns>Awaitable</returns>
        public async ValueTask OnMessageAsync(OrderCreatedEvent message)
        {
            await this.fulfilment.FulfilOrderAsync(message.Id);
        }
    }
}
