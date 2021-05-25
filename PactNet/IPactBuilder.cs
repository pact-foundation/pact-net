using System;
using PactNet.Models;

namespace PactNet
{
    /// <summary>
    /// Build a pact between a consumer and a provider
    /// </summary>
    public interface IPactBuilder
    {
        /// <summary>
        /// Establish the consumer name
        /// </summary>
        /// <param name="consumerName">Name of the consumer</param>
        /// <returns>Same pact builder</returns>
        IPactBuilder ServiceConsumer(string consumerName);

        /// <summary>
        /// Establish the provider name
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <returns>Same pact builder</returns>
        IPactBuilder HasPactWith(string providerName);

        /// <summary>
        /// Start a mock server running locally to build up the pact
        /// </summary>
        /// <param name="port">Port for the mock server. If null, one will be assigned automatically</param>
        /// <param name="host">Host for the mock server</param>
        /// <returns>Interaction builder</returns>
        IInteractionBuilder UsingNativeBackend(int? port = null, IPAddress host = IPAddress.Loopback);

        /// <summary>
        /// Use an existing remote server running at the given URI
        /// </summary>
        /// <param name="uri">Remote server URI</param>
        /// <returns>Interaction builder</returns>
        IInteractionBuilder UsingRemoteBackend(Uri uri);

        /// <summary>
        /// After all interactions are complete, write the pact file
        /// </summary>
        void Build();
    }
}