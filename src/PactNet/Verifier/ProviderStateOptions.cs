using System;

namespace PactNet.Verifier
{
    /// <summary>
    /// Provider state options
    /// </summary>
    internal class ProviderStateOptions : IProviderStateOptions
    {
        private readonly IVerifierProvider provider;
        private readonly Uri providerStateUri;

        private bool teardown = false;
        private ProviderStateStyle style = ProviderStateStyle.Body;

        /// <summary>
        /// Initialises a new instance of the <see cref="ProviderStateOptions"/> class.
        /// </summary>
        /// <param name="provider">Pact verifier provider</param>
        /// <param name="providerStateUri">Provider states URI</param>
        public ProviderStateOptions(IVerifierProvider provider, Uri providerStateUri)
        {
            this.provider = provider;
            this.providerStateUri = providerStateUri;
        }

        /// <summary>
        /// Provide request callbacks after each interaction is verified
        /// </summary>
        /// <returns>Fluent builder</returns>
        public IProviderStateOptions WithTeardown()
        {
            this.teardown = true;
            return this;
        }

        /// <summary>
        /// Configure the style in which the provider state endpoint is invoked
        /// </summary>
        /// <param name="style">Provider state style</param>
        /// <returns>Fluent builder</returns>
        public IProviderStateOptions WithStyle(ProviderStateStyle style)
        {
            this.style = style;
            return this;
        }

        /// <summary>
        /// Apply the configured values to the verifier
        /// </summary>
        public void Apply()
        {
            this.provider.SetProviderState(providerStateUri, this.teardown, this.style == ProviderStateStyle.Body);
        }
    }
}
