using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PactNet.Internal;

namespace PactNet.Verifier
{
    /// <summary>
    /// Native pact broker options
    /// </summary>
    internal class PactBrokerOptions : IPactBrokerOptions
    {
        private static readonly JsonSerializerSettings ConsumerSelectorSettings = new()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly IVerifierProvider provider;
        private readonly Uri brokerUri;

        private string username;
        private string password;
        private string token;
        private bool enablePending;
        private DateTime? includeWipPactsSince;
        private ICollection<string> consumerVersionTags = Array.Empty<string>();
        private ICollection<string> consumerVersionSelectors = Array.Empty<string>();

        /// <summary>
        /// Initialises a new instance of the <see cref="PactBrokerOptions"/> class.
        /// </summary>
        /// <param name="provider">Pact verifier provider</param>
        /// <param name="brokerUri">Pact broker URI</param>
        public PactBrokerOptions(IVerifierProvider provider, Uri brokerUri)
        {
            this.provider = provider;
            this.brokerUri = brokerUri;
        }

        /// <summary>
        /// Use Basic authentication with the Pact Broker
        /// </summary>
        /// <param name="username">Pact broker username</param>
        /// <param name="password">Pact broker password</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions BasicAuthentication(string username, string password)
        {
            Guard.NotNullOrEmpty(username, nameof(username));
            Guard.NotNullOrEmpty(password, nameof(password));

            this.username = username;
            this.password = password;

            return this;
        }

        /// <summary>
        /// Use Token authentication with the Pact Broker
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions TokenAuthentication(string token)
        {
            Guard.NotNullOrEmpty(token, nameof(token));

            this.token = token;

            return this;
        }

        /// <summary>
        /// Enable pending pacts
        /// </summary>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions EnablePending()
        {
            this.enablePending = true;

            return this;
        }

        /// <summary>
        /// Consumer tag versions to retrieve
        /// </summary>
        /// <param name="tags">Consumer tags</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions ConsumerTags(params string[] tags)
        {
            this.consumerVersionTags = tags;

            return this;
        }

        /// <summary>
        /// Consumer version selectors to control which pacts are returned from the broker
        /// </summary>
        /// <param name="selectors">Consumer version selectors</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>See <see href="https://docs.pact.io/pact_broker/advanced_topics/consumer_version_selectors"/></remarks>
        public IPactBrokerOptions ConsumerVersionSelectors(params ConsumerVersionSelector[] selectors)
        {
            string[] serialised = selectors.Select(s => JsonConvert.SerializeObject(s, ConsumerSelectorSettings)).ToArray();

            this.consumerVersionSelectors = serialised;

            return this;
        }

        /// <summary>
        /// Include WIP pacts since the given date
        /// </summary>
        /// <param name="date">WIP cut-off date</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions IncludeWipPactsSince(DateTime date)
        {
            this.includeWipPactsSince = date;

            return this;
        }

        /// <summary>
        /// Publish results to the pact broker
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="configure">Configure the publish options</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions PublishResults(string providerVersion, Action<IPactBrokerPublishOptions> configure)
        {
            Guard.NotNullOrEmpty(providerVersion, nameof(providerVersion));
            Guard.NotNull(configure, nameof(configure));

            var options = new PactBrokerPublishOptions(this.provider, providerVersion);
            configure.Invoke(options);
            options.Apply();

            return this;
        }

        /// <summary>
        /// Finalise the configuration with the provider
        /// </summary>
        public void Apply()
        {
            this.provider.AddBrokerSource(brokerUri,
                                          null, // TODO: where do we get the provider name from again?
                                          this.username,
                                          this.password,
                                          this.token,
                                          this.enablePending,
                                          this.includeWipPactsSince,
                                          Array.Empty<string>(), // TODO: What does the provider tags arg do?
                                          null,                  // TODO: What does the provider branch arg do?
                                          this.consumerVersionSelectors,
                                          this.consumerVersionTags);
        }
    }
}
