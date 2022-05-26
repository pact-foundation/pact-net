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
        private string providerBranch;
        private ICollection<string> providerTags = Array.Empty<string>();
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
        /// Set the provider branch for retrieving pacts
        /// </summary>
        /// <param name="branch">Branch name</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions ProviderBranch(string branch)
        {
            this.providerBranch = branch;
            return this;
        }

        /// <summary>
        /// Set the provider tags for retrieving pacts
        /// </summary>
        /// <param name="tags">Tags</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions ProviderTags(params string[] tags)
        {
            this.providerTags = tags;
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
        public IPactBrokerOptions ConsumerVersionSelectors(ICollection<ConsumerVersionSelector> selectors)
        {
            string[] serialised = selectors.Select(s => JsonConvert.SerializeObject(s, ConsumerSelectorSettings)).ToArray();

            this.consumerVersionSelectors = serialised;

            return this;
        }

        /// <summary>
        /// Consumer version selectors to control which pacts are returned from the broker
        /// </summary>
        /// <param name="selectors">Consumer version selectors</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>See <see href="https://docs.pact.io/pact_broker/advanced_topics/consumer_version_selectors"/></remarks>
        public IPactBrokerOptions ConsumerVersionSelectors(params ConsumerVersionSelector[] selectors)
            => this.ConsumerVersionSelectors((ICollection<ConsumerVersionSelector>)selectors);

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
        /// Publish results to the pact broker without any additional settings
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions PublishResults(string providerVersion)
            => this.PublishResults(providerVersion, _ => { });

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
        /// Publish results to the pact broker without any additional settings, if the condition is met
        /// </summary>
        /// <param name="condition">Only publish if this condition is true</param>
        /// <param name="providerVersion">Provider version</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions PublishResults(bool condition, string providerVersion)
            => this.PublishResults(condition, providerVersion, _ => { });

        /// <summary>
        /// Publish results to the pact broker if the condition is met
        /// </summary>
        /// <param name="condition">Only publish if this condition is true</param>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="configure">Configure the publish options</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions PublishResults(bool condition, string providerVersion, Action<IPactBrokerPublishOptions> configure)
            => condition ? this.PublishResults(providerVersion, configure) : this;

        /// <summary>
        /// Finalise the configuration with the provider
        /// </summary>
        public void Apply()
        {
            this.provider.AddBrokerSource(brokerUri,
                                          this.username,
                                          this.password,
                                          this.token,
                                          this.enablePending,
                                          this.includeWipPactsSince,
                                          this.providerTags,
                                          this.providerBranch,
                                          this.consumerVersionSelectors,
                                          this.consumerVersionTags);
        }
    }
}
