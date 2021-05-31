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
        public PactVerifier(PactVerifierConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Set up the provider state setup path so the service can configure states
        /// </summary>
        /// <param name="providerStatePath">Provider state setup path</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier ProviderState(string providerStatePath)
        {
            this.verifierArgs.Add("--state-change-url");
            this.verifierArgs.Add(providerStatePath);
            return this;
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
        public IPactVerifier PactFile(FileInfo pactFile)
        {
            this.verifierArgs.Add("--file");
            this.verifierArgs.Add(pactFile.FullName);
            return this;
        }

        /// <summary>
        /// Verify a pact from a URI
        /// </summary>
        /// <param name="pactUri">Pact file URI</param>
        /// <param name="options">Pact URI options</param>
        /// <param name="providerVersionTags">Provider version tags</param>
        /// <returns>Fluent builder</returns>
        public IPactVerifier PactUri(Uri pactUri, PactUriOptions options = null, IEnumerable<string> providerVersionTags = null)
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
        public IPactVerifier PactBroker(Uri brokerBaseUri,
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
        /// Verify provider interactions
        /// </summary>
        /// <param name="description">Interaction description. All interactions are verified if this is null</param>
        /// <param name="providerState">Provider state description. All provider states are verified if this is null</param>
        public void Verify(string description = null, string providerState = null)
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

            if (this.config.PublishVerificationResults)
            {
                if (string.IsNullOrWhiteSpace(this.config.ProviderVersion))
                {
                    throw new InvalidOperationException("Can't publish verification results without the provider version being set");
                }

                this.verifierArgs.Add("--publish");
                this.verifierArgs.Add("--provider-version");
                this.verifierArgs.Add(this.config.ProviderVersion);

                if (this.config.ProviderTags.Any())
                {
                    string tags = string.Join(",", this.config.ProviderTags);
                    this.verifierArgs.Add("--provider-tags");
                    this.verifierArgs.Add(tags);
                }
            }

            // TODO: make verifier log level configurable
            this.verifierArgs.Add("--loglevel");
            this.verifierArgs.Add("trace");

            string args = string.Join(Environment.NewLine, this.verifierArgs);

            this.config.WriteLine("Invoking the pact verifier with args:");
            this.config.WriteLine(args);

            int result = PactVerifierInterop.Verify(args);

            switch (result)
            {
                case 0: break;
                case 1: throw new PactFailureException("The verification process failed, see output for errors");
                case 2: throw new PactFailureException("A null pointer was received");
                case 3: throw new PactFailureException("The method panicked");
                case 4: throw new PactFailureException("Invalid arguments were provided to the verification process");
                default: throw new PactFailureException($"An unknown error occurred with error code {result}");
            }
        }
    }
}