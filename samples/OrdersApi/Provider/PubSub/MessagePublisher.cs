using System.Threading.Tasks;

namespace Provider.PubSub
{
    /// <summary>
    /// Message publisher
    /// </summary>
    public class MessagePublisher : IMessagePublisher
    {
        /// <summary>
        /// Publish a message
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="message">Message to publish</param>
        /// <returns>Awaitable</returns>
        public ValueTask PublishAsync<T>(T message)
        {
            // NOTE: For this demo we don't do anything, but a real implementation would publish to Kafka/RabbitMQ/etc

            return ValueTask.CompletedTask;
        }
    }
}
