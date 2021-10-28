using System;

namespace PactNet.Verifier
{
    /// <summary>
    /// Native pact verifier source type state
    /// </summary>
    internal class PactVerifierSource : IPactVerifierSource
    {
        private readonly IVerifierArguments verifierArgs;
        private readonly IVerifierProvider verifier;
        private readonly PactVerifierConfig config;

        /// <summary>
        /// Initialises a new instance of the <see cref="PactNet.Verifier.PactVerifierSource"/> class.
        /// </summary>
        /// <param name="verifierArgs">Verifier args to populate</param>
        /// <param name="verifier">Pact verifier provider</param>
        /// <param name="config">Verifier config</param>
        internal PactVerifierSource(IVerifierArguments verifierArgs, IVerifierProvider verifier, PactVerifierConfig config)
        {
            this.verifierArgs = verifierArgs;
            this.verifier = verifier;
            this.config = config;
        }

        /// <summary>
        /// Set up the provider state setup URI so the service can configure states
        /// </summary>
        /// <param name="providerStateUri">Provider state URI</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithProviderStateUrl(Uri providerStateUri)
        {
            this.verifierArgs.AddOption("--state-change-url", providerStateUri.ToString(), nameof(providerStateUri));

            return this;
        }

        /// <summary>
        /// Filter the interactions to only those matching the given description and/or provider state
        /// </summary>
        /// <param name="description">Interaction description. All interactions are verified if this is null</param>
        /// <param name="providerState">Provider state description. All provider states are verified if this is null</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithFilter(string description = null, string providerState = null)
        {
            // TODO: Allow env vars to specify description and provider state filters like the old version did
            if (!string.IsNullOrWhiteSpace(description))
            {
                this.verifierArgs.AddOption("--filter-description", description, nameof(description));
            }

            if (!string.IsNullOrWhiteSpace(providerState))
            {
                this.verifierArgs.AddOption("--filter-state", providerState, nameof(providerState));
            }

            return this;
        }

        /// <summary>
        /// Alter the log level from the default value
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifierSource WithLogLevel(PactLogLevel level)
        {
            string arg = level switch
            {
                PactLogLevel.Trace => "trace",
                PactLogLevel.Debug => "debug",
                PactLogLevel.Information => "info",
                PactLogLevel.Warn => "warn",
                PactLogLevel.Error => "error",
                PactLogLevel.None => "none",
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Unsupported log level")
            };

            this.verifierArgs.AddOption("--loglevel", arg);

            return this;
        }

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        public void Verify()
        {
            string formatted = this.verifierArgs.ToString();

            this.config.WriteLine("Invoking the pact verifier with args:");
            this.config.WriteLine(formatted);

            this.verifier.Verify(formatted);
        }
    }
}
