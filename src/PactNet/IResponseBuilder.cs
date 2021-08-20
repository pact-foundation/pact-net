using System.Net;
using Newtonsoft.Json;

namespace PactNet
{
    /// <summary>
    /// Mock response builder for a v2 messagePact
    /// </summary>
    public interface IResponseBuilderV2
    {
        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 WithStatus(HttpStatusCode status);

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 WithStatus(ushort status);

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 WithHeader(string key, string value);

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 WithJsonBody(dynamic body);

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 WithJsonBody(dynamic body, JsonSerializerSettings settings);
    }

    /// <summary>
    /// Mock response builder for a v3 messagePact
    /// </summary>
    public interface IResponseBuilderV3
    {
        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 WithStatus(HttpStatusCode status);

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 WithStatus(ushort status);

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 WithHeader(string key, string value);

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 WithJsonBody(dynamic body);

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 WithJsonBody(dynamic body, JsonSerializerSettings settings);
    }
}
