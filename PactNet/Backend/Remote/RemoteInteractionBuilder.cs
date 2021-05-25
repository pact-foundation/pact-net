using System;

namespace PactNet.Backend.Remote
{
    /// <summary>
    /// Remote interaction builder
    /// </summary>
    public class RemoteInteractionBuilder : IInteractionBuilder
    {
        /// <summary>
        /// URI for the mock provider
        /// </summary>
        public Uri MockProviderUri { get; }

        /// <summary>
        /// Start a new interaction and configure the request
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder UponReceiving(string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies all configured interactions
        /// </summary>
        /// <exception cref="PactFailureException">Verification failed</exception>
        public void Verify()
        {
            throw new NotImplementedException();
        }
    }
}