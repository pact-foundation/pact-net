using System;

namespace PactNet
{
    /// <summary>
    /// Pact
    /// </summary>
    public class Pact : IPact
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
        /// Specification version
        /// </summary>
        public string SpecificationVersion { get; }

        /// <summary>
        /// Pact config
        /// </summary>
        public PactConfig Config { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="Pact"/> class.
        /// </summary>
        /// <param name="consumer">Name of the consumer</param>
        /// <param name="provider">Name of the provider</param>
        /// <param name="specificationVersion">Specification version</param>
        public Pact(string consumer, string provider, string specificationVersion)
            : this(
                consumer,
                provider,
                specificationVersion,
                new PactConfig())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Pact"/> class.
        /// </summary>
        /// <param name="consumer">Name of the consumer</param>
        /// <param name="provider">Name of the provider</param>
        /// <param name="specificationVersion">Specification version</param>
        /// <param name="config">Pact config</param>
        public Pact(string consumer, string provider, string specificationVersion, PactConfig config)
        {
            if (string.IsNullOrWhiteSpace(consumer))
            {
                throw new ArgumentException("Please provide a valid consumer name", nameof(consumer));
            }

            if (string.IsNullOrWhiteSpace(provider))
            {
                throw new ArgumentException("Please provide a valid provider name", nameof(provider));
            }

            if (string.IsNullOrWhiteSpace(specificationVersion))
            {
                throw new ArgumentException("Please provide a valid specification version", nameof(specificationVersion));
            }

            this.Consumer = consumer;
            this.Provider = provider;
            this.SpecificationVersion = specificationVersion;
            this.Config = config;
        }
    }
}