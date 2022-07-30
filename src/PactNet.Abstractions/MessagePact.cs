using System;

namespace PactNet
{
    /// <summary>
    /// Pact
    /// </summary>
    [Obsolete("Use Pact instead")]
    public class MessagePact : IMessagePactV3
    {
        /// <summary>
        /// Consumer name
        /// </summary>
        public string Consumer { get; }

        /// <summary>
        /// Provider name
        /// </summary>
        public string Provider { get; }

        /// <summary>
        /// Pact config
        /// </summary>
        public PactConfig Config { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Pact"/> class.
        /// </summary>
        /// <param name="consumer">Name of the consumer</param>
        /// <param name="provider">Name of the provider</param>
        private MessagePact(string consumer, string provider)
            : this(
                consumer,
                provider,
                new PactConfig())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Pact"/> class.
        /// </summary>
        /// <param name="consumer">Name of the consumer</param>
        /// <param name="provider">Name of the provider</param>
        /// <param name="config">Pact config</param>
        private MessagePact(string consumer, string provider, PactConfig config)
        {
            if (string.IsNullOrWhiteSpace(consumer))
            {
                throw new ArgumentException("Please provide a valid consumer name", nameof(consumer));
            }

            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentException("Please provide a valid provider name", nameof(provider));
            }

            this.Consumer = consumer;
            this.Provider = provider;
            this.Config = config;
        }

        /// <summary>
        /// Create a new v3 messagePact
        /// </summary>
        /// <param name="consumer">Name of the consumer</param>
        /// <param name="provider">Name of the provider</param>
        /// <returns>v2 Pact</returns>
        [Obsolete("Use Pact.V3 instead")]
        public static IMessagePactV3 V3(string consumer, string provider)
        {
            return new MessagePact(consumer, provider);
        }

        /// <summary>
        /// Create a new v3 messagePact
        /// </summary>
        /// <param name="consumer">Name of the consumer</param>
        /// <param name="provider">Name of the provider</param>
        /// <param name="config">Pact config</param>
        /// <returns>v3 Pact</returns>
        [Obsolete("Use Pact.V3 instead")]
        public static IMessagePactV3 V3(string consumer, string provider, PactConfig config)
        {
            return new MessagePact(consumer, provider, config);
        }
    }
}
