using System;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous HTTP interactions
    /// </summary>
    internal interface ISynchronousHttpDriver : IInteractionDriver, ICompletedPactDriver
    {
        /// <summary>
        /// Create the mock server for the given pact handle
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="addrStr">Host and port for the mock server</param>
        /// <param name="tls">Enable TLS</param>
        /// <returns>Mock server port</returns>
        /// <exception cref="InvalidOperationException">Failed to start mock server</exception>
        int CreateMockServerForPact(PactHandle pact, string addrStr, bool tls);

        /// <summary>
        /// Get a string representing the mismatches following interaction testing
        /// </summary>
        /// <param name="mockServerPort">Mock server port</param>
        /// <returns>Mismatch string</returns>
        string MockServerMismatches(int mockServerPort);

        /// <summary>
        /// Get a string representing the mock server logs following interaction testing
        /// </summary>
        /// <param name="mockServerPort">Mock server port</param>
        /// <returns>Log string</returns>
        string MockServerLogs(int mockServerPort);

        /// <summary>
        /// Clean up the mock server following consumer testing
        /// </summary>
        /// <param name="mockServerPort">Mock server port</param>
        /// <returns>Cleanup successful</returns>
        bool CleanupMockServer(int mockServerPort);

        /// <summary>
        /// Add a request to the interaction
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Success</returns>
        bool WithRequest(InteractionHandle interaction, string method, string path);

        /// <summary>
        /// Add a query string parameter to the interaction
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="name">Query string parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="index">Parameter index (for if the same name is used multiple times)</param>
        /// <returns>Success</returns>
        bool WithQueryParameter(InteractionHandle interaction, string name, string value, uint index);

        /// <summary>
        /// Set a request header
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="name">header name</param>
        /// <param name="value">Header value</param>
        /// <param name="index">Header index (for if the same header is added multiple times)</param>
        /// <returns>Success</returns>
        bool WithRequestHeader(InteractionHandle interaction, string name, string value, uint index);

        /// <summary>
        /// Set a response header
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="name">header name</param>
        /// <param name="value">Header value</param>
        /// <param name="index">Header index (for if the same header is added multiple times)</param>
        /// <returns>Success</returns>
        bool WithResponseHeader(InteractionHandle interaction, string name, string value, uint index);

        /// <summary>
        /// Set the response status
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="status">Status code</param>
        /// <returns>Success</returns>
        bool ResponseStatus(InteractionHandle interaction, ushort status);

        /// <summary>
        /// Set the request body
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="contentType">Context type</param>
        /// <param name="body">Serialised body</param>
        /// <returns>Success</returns>
        bool WithRequestBody(InteractionHandle interaction, string contentType, string body);

        /// <summary>
        /// Set the response body
        /// </summary>
        /// <param name="interaction">Interaction</param>
        /// <param name="contentType">Context type</param>
        /// <param name="body">Serialised body</param>
        /// <returns>Success</returns>
        bool WithResponseBody(InteractionHandle interaction, string contentType, string body);
    }
}
