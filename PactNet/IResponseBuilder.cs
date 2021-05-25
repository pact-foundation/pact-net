using System.Net;
using Newtonsoft.Json;

namespace PactNet
{
    /// <summary>
    /// Mock response builder
    /// </summary>
    public interface IResponseBuilder
    {
        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilder WithStatus(HttpStatusCode status);

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilder WithStatus(ushort status);

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilder WithHeader(string key, string value);

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilder WithJsonBody(dynamic body);

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings);

        // TODO: Support binary and multi-part body
    }
}