using System;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact broker options
    /// </summary>
    public interface IPactBrokerOptions
    {
        /// <summary>
        /// Use Basic authentication with the Pact Broker
        /// </summary>
        /// <param name="username">Pact broker username</param>
        /// <param name="password">Pact broker password</param>
        /// <returns>Fluent builder</returns>
        IPactBrokerOptions BasicAuthentication(string username, string password);

        /// <summary>
        /// Use Token authentication with the Pact Broker
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>Fluent builder</returns>
        IPactBrokerOptions TokenAuthentication(string token);

        /// <summary>
        /// Enable pending pacts
        /// </summary>
        /// <returns>Fluent builder</returns>
        IPactBrokerOptions EnablePending();

        /// <summary>
        /// Consumer tag versions to retrieve
        /// </summary>
        /// <param name="tags">Consumer tags</param>
        /// <returns>Fluent builder</returns>
        IPactBrokerOptions ConsumerTags(params string[] tags);

        /// <summary>
        /// Consumer version selectors to control which pacts are returned from the broker
        /// </summary>
        /// <param name="selectors">Consumer version selectors</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>See <see href="https://docs.pact.io/pact_broker/advanced_topics/consumer_version_selectors"/></remarks>
        IPactBrokerOptions ConsumerVersionSelectors(params ConsumerVersionSelector[] selectors);

        /// <summary>
        /// Include WIP pacts since the given date
        /// </summary>
        /// <param name="date">WIP cut-off date</param>
        /// <returns>Fluent builder</returns>
        IPactBrokerOptions IncludeWipPactsSince(DateTime date);

        /// <summary>
        /// Publish results to the pact broker
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="tags">Optional tags to add to the verification</param>
        /// <returns>Fluent builder</returns>
        IPactBrokerOptions PublishResults(string providerVersion, params string[] tags);
    }
}
