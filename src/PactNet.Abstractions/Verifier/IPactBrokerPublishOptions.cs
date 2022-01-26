using System;

namespace PactNet.Verifier
{
    /// <summary>
    /// Options for publishing verification results to the Pact Broker
    /// </summary>
    public interface IPactBrokerPublishOptions
    {
        /// <summary>
        /// Tag the provider with the given tags
        /// </summary>
        /// <param name="tags">Tags to apply</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerPublishOptions ProviderTags(params string[] tags);

        /// <summary>
        /// Set the branch of the provider
        /// </summary>
        /// <param name="branch">Provider branch</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerPublishOptions ProviderBranch(string branch);

        /// <summary>
        /// URI of the build that performed the verification
        /// </summary>
        /// <param name="uri">Build URI</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerPublishOptions BuildUri(Uri uri);
    }
}
