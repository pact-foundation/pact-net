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
        /// Publish results to the pact broker without any additional settings
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions PublishResults(string providerVersion);

        /// <summary>
        /// Publish results to the pact broker with additional settings such as provider branch
        /// </summary>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="configure">Configure the publish options</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions PublishResults(string providerVersion, Action<IPactBrokerPublishOptions> configure);

        /// <summary>
        /// Publish results to the pact broker without any additional settings, if the condition is met
        /// </summary>
        /// <param name="condition">Only publish if this condition is true</param>
        /// <param name="providerVersion">Provider version</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions PublishResults(bool condition, string providerVersion);

        /// <summary>
        /// Publish results to the pact broker with additional settings such as provider branch, if the condition is met
        /// </summary>
        /// <param name="condition">Only publish if this condition is true</param>
        /// <param name="providerVersion">Provider version</param>
        /// <param name="configure">Configure the publish options</param>
        /// <returns>Fluent builder</returns>
        IPactUriOptions PublishResults(bool condition, string providerVersion, Action<IPactBrokerPublishOptions> configure);
    }
}
