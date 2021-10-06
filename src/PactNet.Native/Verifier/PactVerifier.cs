using System;
using System.Collections.Generic;
using PactNet.Native.Internal;
using PactNet.Verifier;

namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Pact verifier
    /// </summary>
    public class PactVerifier : IPactVerifier
    {
        private readonly PactVerifierConfig config;
        private readonly IDictionary<string, string> verifierArgs;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        public PactVerifier() : this(new PactVerifierConfig())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="config">Pact verifier config</param>
        public PactVerifier(PactVerifierConfig config) : this(new Dictionary<string, string>(), config)
        {
            Guard.NotNull(config, nameof(config));
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="verifierArgs">Pact verifier args</param>
        /// <param name="config">Pact verifier config</param>
        internal PactVerifier(IDictionary<string, string> verifierArgs, PactVerifierConfig config)
        {
            this.config = config;
            this.verifierArgs = verifierArgs;
        }

        /// <summary>
        /// Set the provider details
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierProvider ServiceProvider(string providerName, Uri pactUri)
        {
            Guard.NotNull(pactUri, nameof(pactUri));

            this.verifierArgs.AddOption("--provider-name", providerName);
            this.verifierArgs.AddOption("--hostname", pactUri.Host);
            this.verifierArgs.AddOption("--port", pactUri.Port.ToString());

            if (pactUri.AbsolutePath != "/")
            {
                this.verifierArgs.Add("--base-path", pactUri.AbsolutePath);
            }

            return new NativePactVerifierProvider(this.verifierArgs, this.config);
        }
    }
}
