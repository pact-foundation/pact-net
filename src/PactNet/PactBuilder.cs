using PactNet.Drivers.Http;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Pact builder for the native backend
    /// </summary>
    internal class PactBuilder : AbstractPactBuilder, IPactBuilderV2, IPactBuilderV3, IPactBuilderV4
    {
        private readonly IHttpPactDriver pact;
        private readonly PactConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactBuilder"/> class.
        /// </summary>
        /// <param name="pact">Pact driver</param>
        /// <param name="config">Pact config</param>
        /// <param name="port">Optional port, otherwise one is dynamically allocated</param>
        /// <param name="host">Optional host, otherwise loopback is used</param>
        internal PactBuilder(IHttpPactDriver pact,
                             PactConfig config,
                             int? port = null,
                             IPAddress host = IPAddress.Loopback)
            : base(pact, config, port, host, "http")
        {
            this.pact = pact;
            this.config = config;
        }

        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IPactBuilderV2.UponReceiving(string description)
            => this.UponReceiving(description);

        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IPactBuilderV3.UponReceiving(string description)
            => this.UponReceiving(description);

        /// <summary>
        /// Add a new interaction to the pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 IPactBuilderV4.UponReceiving(string description)
            => this.UponReceiving(description);

        /// <summary>
        /// Create a new request/response interaction
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Request builder</returns>
        internal RequestBuilder UponReceiving(string description)
        {
            IHttpInteractionDriver interactions = this.pact.NewHttpInteraction(description);

            var requestBuilder = new RequestBuilder(interactions, this.config.DefaultJsonSettings);
            return requestBuilder;
        }
    }
}
