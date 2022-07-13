using System;

namespace PactNet.Verifier
{
    /// <summary>
    /// Options for URI pact sources
    /// </summary>
    public interface IPactUriOptions
    {
        /// <summary>
        /// Use Basic authentication to access the URI
        /// </summary>
        /// <param name="username">Pact broker username</param>
        /// <param name="password">Pact broker password</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions BasicAuthentication(string username, string password);

        /// <summary>
        /// Use Token authentication to access the URI
        /// </summary>
        /// <param name="token">Auth token</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions TokenAuthentication(string token);

        /// <summary>
        /// Publish results to the pact broker
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="configure">Configure the publish options</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions PublishResults(string providerVersion, Action<IPactBrokerPublishOptions> configure);

        /// <summary>
        /// Publish results to the pact broker if the condition is met
        /// </summary>
        /// <param name="condition">Only publish if this condition is true</param>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="configure">Configure the publish options</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions PublishResults(bool condition, string providerVersion, Action<IPactBrokerPublishOptions> configure);
    }
}
