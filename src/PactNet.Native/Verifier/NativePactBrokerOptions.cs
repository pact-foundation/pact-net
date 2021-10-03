using System.Collections.Generic;
using System.Linq;
using PactNet.Native.Internal;
using PactNet.Verifier;

namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Native pact broker options
    /// </summary>
    internal class NativePactBrokerOptions : IPactBrokerOptions
    {
        private readonly IDictionary<string, string> verifierArgs;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativePactBrokerOptions"/> class.
        /// </summary>
        /// <param name="verifierArgs">Pact verifier args</param>
        public NativePactBrokerOptions(IDictionary<string, string> verifierArgs)
        {
            this.verifierArgs = verifierArgs;
        }

        /// <summary>
        /// Use Basic authentication with the Pact Broker
        /// </summary>
        /// <param name="username">Pact broker username</param>
        /// <param name="password">Pact broker password</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions BasicAuthentication(string username, string password)
        {
            this.verifierArgs.AddOption("--user", username, nameof(username));
            this.verifierArgs.AddOption("--password", password, nameof(password));

            return this;
        }

        /// <summary>
        /// Use Token authentication with the Pact Broker
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions TokenAuthentication(string token)
        {
            this.verifierArgs.AddOption("--token", token, nameof(token));

            return this;
        }

        /// <summary>
        /// Enable pending pacts
        /// </summary>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions EnablePending()
        {
            this.verifierArgs.AddFlag("--enable-pending");

            return this;
        }

        /// <summary>
        /// Consumer tag versions to retrieve
        /// </summary>
        /// <param name="tags">Consumer tags</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions ConsumerTags(params string[] tags)
        {
            if (tags.Any())
            {
                string versions = string.Join(",", tags);
                this.verifierArgs.AddOption("--consumer-version-tags", versions);
            }

            return this;
        }

        /// <summary>
        /// Include WIP pacts since the given date
        /// </summary>
        /// <param name="date">WIP cut-off date</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions IncludeWipPactsSince(string date)
        {
            this.verifierArgs.AddOption("--include-wip-pacts-since", date, nameof(date));

            return this;
        }

        /// <summary>
        /// Publish results to the pact broker
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="tags">Optional tags to add to the verification</param>
        /// <returns>Fluent builder</returns>
        public IPactBrokerOptions PublishResults(string providerVersion, params string[] tags)
        {
            this.verifierArgs.AddFlag("--publish");
            this.verifierArgs.AddOption("--provider-version", providerVersion, nameof(providerVersion));

            if (tags.Any())
            {
                string formatted = string.Join(",", tags);
                this.verifierArgs.AddOption("--provider-tags", formatted);
            }

            return this;
        }
    }
}
