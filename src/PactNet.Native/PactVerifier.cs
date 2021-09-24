using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PactNet.Native
{
    /// <summary>
    /// Pact verifier
    /// </summary>
    public class PactVerifier : IPactVerifier
    {
        private readonly IVerifierProvider verifier;
        private readonly PactVerifierConfig config;
        private readonly IList<string> verifierArgs = new List<string>();

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        public PactVerifier() : this(new PactVerifierConfig())
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="config">Verifier configuration</param>
        public PactVerifier(PactVerifierConfig config) : this(new NativePactVerifier(), config)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactVerifier"/> class.
        /// </summary>
        /// <param name="verifier">Verifier provider</param>
        /// <param name="config">Config</param>
        internal PactVerifier(IVerifierProvider verifier, PactVerifierConfig config)
        {
            this.verifier = verifier;
            this.config = config;
        }

        /// <summary>
        /// Set the provider details
        /// </summary>
        /// <param name="providerName">Name of the provider</param>
        /// <param name="pactUri">URI of the running service</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier ServiceProvider(string providerName, Uri pactUri)
        {
            this.verifierArgs.Add("--provider-name");
            this.verifierArgs.Add(providerName);
            this.verifierArgs.Add("--hostname");
            this.verifierArgs.Add(pactUri.Host);
            this.verifierArgs.Add("--port");
            this.verifierArgs.Add(pactUri.Port.ToString());

            if (pactUri.AbsolutePath != "/")
            {
                this.verifierArgs.Add("--base-path");
                this.verifierArgs.Add(pactUri.AbsolutePath);
            }

            return this;
        }

        /// <summary>
        /// Set the consumer name
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier HonoursPactWith(string consumerName)
        {
            this.verifierArgs.Add("--filter-consumer");
            this.verifierArgs.Add(consumerName);
            return this;
        }

        /// <summary>
        /// Verify a pact file directly
        /// </summary>
        /// <param name="pactFile">Pact file path</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier FromPactFile(FileInfo pactFile)
        {
            this.verifierArgs.Add("--file");
            this.verifierArgs.Add(pactFile.FullName);
            return this;
        }

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier FromPactUri(Uri pactUri)
        {
            this.verifierArgs.Add("--url");
            this.verifierArgs.Add(pactUri.ToString());
            return this;
        }

        /// <summary>
        /// Use the pact broker to retrieve pact files
        /// </summary>
        /// <param name="brokerBaseUri">Base URI for the broker</param>
        /// <param name="uriOptions">Options for calling the pact broker</param>
        /// <param name="enablePending">Enable pending pacts?</param>
        /// <param name="consumerVersionTags">Consumer tag versions to retrieve</param>
        /// <param name="includeWipPactsSince">Include WIP pacts since the given filter</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier FromPactBroker(Uri brokerBaseUri,
                                        PactUriOptions uriOptions = null,
                                        bool enablePending = false,
                                        IEnumerable<string> consumerVersionTags = null,
                                        string includeWipPactsSince = null)
        {
            this.verifierArgs.Add("--broker-url");
            this.verifierArgs.Add(brokerBaseUri.ToString());

            if (uriOptions != null)
            {
                if (!string.IsNullOrWhiteSpace(uriOptions.Username))
                {
                    this.verifierArgs.Add($"--user");
                    this.verifierArgs.Add(uriOptions.Username);
                }

                if (!string.IsNullOrWhiteSpace(uriOptions.Password))
                {
                    this.verifierArgs.Add("--password");
                    this.verifierArgs.Add(uriOptions.Password);
                }

                if (!string.IsNullOrWhiteSpace(uriOptions.Token))
                {
                    this.verifierArgs.Add("--token");
                    this.verifierArgs.Add(uriOptions.Token);
                }
            }

            if (enablePending)
            {
                this.verifierArgs.Add("--enable-pending");
            }

            if (consumerVersionTags != null && consumerVersionTags.Any())
            {
                string versions = string.Join(",", consumerVersionTags);
                this.verifierArgs.Add("--consumer-version-tags");
                this.verifierArgs.Add(versions);
            }

            if (!string.IsNullOrWhiteSpace(includeWipPactsSince))
            {
                this.verifierArgs.Add("--include-wip-pacts-since");
                this.verifierArgs.Add(includeWipPactsSince);
            }

            return this;
        }

        /// <summary>
        /// Set up the provider state setup URI so the service can configure states
        /// </summary>
        /// <param name="providerStateUri">Provider state URI</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier WithProviderStateUrl(Uri providerStateUri)
        {
            this.verifierArgs.Add("--state-change-url");
            this.verifierArgs.Add(providerStateUri.ToString());
            return this;
        }

        /// <summary>
        /// Filter the interactions to only those matching the given description and/or provider state
        /// </summary>
        /// <param name="description">Interaction description. All interactions are verified if this is null</param>
        /// <param name="providerState">Provider state description. All provider states are verified if this is null</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier WithFilter(string description = null, string providerState = null)
        {
            // TODO: Allow env vars to specify description and provider state filters like the old version did
            if (!string.IsNullOrWhiteSpace(description))
            {
                this.verifierArgs.Add("--filter-description");
                this.verifierArgs.Add(description);
            }

            if (!string.IsNullOrWhiteSpace(providerState))
            {
                this.verifierArgs.Add("--filter-state");
                this.verifierArgs.Add(providerState);
            }

            return this;
        }

        /// <summary>
        /// Publish results to the pact broker
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="providerTags">Optional tags to add to the verification</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier WithPublishedResults(string providerVersion, IEnumerable<string> providerTags = null)
        {
            if (string.IsNullOrWhiteSpace(providerVersion))
            {
                throw new ArgumentException("Can't publish verification results without the provider version being set");
            }

            this.verifierArgs.Add("--publish");
            this.verifierArgs.Add("--provider-version");
            this.verifierArgs.Add(providerVersion);

            if (providerTags != null && providerTags.Any())
            {
                string tags = string.Join(",", providerTags);
                this.verifierArgs.Add("--provider-tags");
                this.verifierArgs.Add(tags);
            }

            return this;
        }

        /// <summary>
        /// Alter the log level from the default value
        /// </summary>
        /// <param name="level">Log level</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier WithLogLevel(PactLogLevel level)
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

            this.verifierArgs.Add("--loglevel");
            this.verifierArgs.Add(arg);

            return this;
        }

        /// <summary>
        /// Verify provider interactions
        /// </summary>
        public void Verify()
        {
            string formatted = string.Join(Environment.NewLine, this.verifierArgs);

            this.config.WriteLine("Invoking the pact verifier with args:");
            this.config.WriteLine(formatted);

            this.verifier.Verify(formatted);
        }
    }
}
