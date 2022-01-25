using System;
using System.Collections.Generic;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact broker publish options
    /// </summary>
    internal class PactBrokerPublishOptions : IPactBrokerPublishOptions
    {
        private readonly IVerifierProvider provider;
        private readonly string version;

        private ICollection<string> tags;
        private Uri buildUri;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactBrokerPublishOptions"/> class.
        /// </summary>
        /// <param name="provider">Pact verifier provider</param>
        /// <param name="version">Provider version</param>
        public PactBrokerPublishOptions(IVerifierProvider provider, string version)
        {
            this.provider = provider;
            this.version = version;
        }

        /// <summary>
        /// Tag the provider with the given tags
        /// </summary>
        /// <param name="tags">Tags to apply</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerPublishOptions ProviderTags(params string[] tags)
        {
            this.tags = tags;
            return this;
        }

        /// <summary>
        /// URI of the build that performed the verification
        /// </summary>
        /// <param name="uri">Build URI</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerPublishOptions BuildUri(Uri uri)
        {
            this.buildUri = uri;
            return this;
        }

        /// <summary>
        /// Apply the configured values to the verifier
        /// </summary>
        public void Apply()
        {
            this.provider.SetVerificationOptions(true,
                                                 this.version,
                                                 this.buildUri,
                                                 false, // TODO: Support disabling SSL verification
                                                 TimeSpan.FromSeconds(5), // TODO: Support request timeouts
                                                 this.tags);
        }
    }
}
