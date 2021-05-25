using System.Net.Http;
using Newtonsoft.Json;

namespace PactNet
{
    /// <summary>
    /// Build up a mock request
    /// </summary>
    public interface IRequestBuilder
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilder Given(string providerState);

        // TODO: Support multiple provider states and provider state params

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilder WithRequest(HttpMethod method, string path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilder WithRequest(string method, string path);

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="value">Query parameter value</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilder WithQuery(string key, string value);

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilder WithHeader(string key, string value);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilder WithJsonBody(dynamic body);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings);

        // TODO: Support binary and multi-part body

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        IResponseBuilder WillRespond();
    }
}