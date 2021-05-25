using System;

namespace PactNet
{
    /// <summary>
    /// Interaction builder
    /// </summary>
    public interface IInteractionBuilder
    {
        /// <summary>
        /// URI for the mock provider
        /// </summary>
        Uri MockProviderUri { get; }

        /// <summary>
        /// Start a new interaction
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilder UponReceiving(string description);

        /// <summary>
        /// Verifies all configured interactions
        /// </summary>
        /// <exception cref="PactFailureException">Verification failed</exception>
        void Verify();
    }
}