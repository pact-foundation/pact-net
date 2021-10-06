using System;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier
    /// </summary>
    public interface IPactVerifier
    {
        /// <summary>
        /// Set the provider details
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierProvider ServiceProvider(string providerName, Uri pactUri);

        /// <summary>
        /// Set the provider details
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <param name="relativePath">The relative path of the provider route</param>
        /// <returns>Fluent builder</returns>
        IPactVerifierProvider ServiceProvider(string providerName, Uri pactUri, string relativePath);
    }
}
