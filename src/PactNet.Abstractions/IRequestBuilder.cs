using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using PactNet.Matchers;

namespace PactNet
{
    /// <summary>
    /// Build up a mock request for a v2 pact
    /// </summary>
    public interface IRequestBuilderV2
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 Given(string providerState);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithRequest(HttpMethod method, string path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithRequest(string method, string path);

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="value">Query parameter value</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilderV2 WithQuery(string key, string value);

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithHeader(string key, string value);

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithHeader(string key, IMatcher matcher);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithJsonBody(dynamic body);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithJsonBody(dynamic body, string contentType);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithJsonBody(dynamic body, JsonSerializerOptions settings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithJsonBody(dynamic body, JsonSerializerOptions settings, string contentType);

        /// <summary>
        /// A pre-formatted body which should be used as-is for the request 
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 WithBody(string body, string contentType);

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        IResponseBuilderV2 WillRespond();
    }

    /// <summary>
    /// Build up a mock request for a v3 pact
    /// </summary>
    public interface IRequestBuilderV3
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 Given(string providerState);

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 Given(string providerState, IDictionary<string, string> parameters);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithRequest(HttpMethod method, string path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithRequest(string method, string path);

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="value">Query parameter value</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilderV3 WithQuery(string key, string value);

        /// <summary>
        /// Add a query parameter matcher
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="matcher">Query parameter matcher</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilderV3 WithQuery(string key, IMatcher matcher);

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithHeader(string key, string value);

        /// <summary>
        /// Add a request header matcher
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithHeader(string key, IMatcher matcher);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithJsonBody(dynamic body);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithJsonBody(dynamic body, string contentType);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithJsonBody(dynamic body, JsonSerializerOptions settings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithJsonBody(dynamic body, JsonSerializerOptions settings, string contentType);

        /// <summary>
        /// A pre-formatted body which should be used as-is for the request 
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 WithBody(string body, string contentType);

        // TODO: Support binary and multi-part body

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        IResponseBuilderV3 WillRespond();
    }

    /// <summary>
    /// Build up a mock request for a v4 pact
    /// </summary>
    public interface IRequestBuilderV4
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 Given(string providerState);

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 Given(string providerState, IDictionary<string, string> parameters);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithRequest(HttpMethod method, string path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithRequest(string method, string path);

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="value">Query parameter value</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilderV4 WithQuery(string key, string value);

        /// <summary>
        /// Add a query parameter matcher
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="matcher">Query parameter matcher</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilderV4 WithQuery(string key, IMatcher matcher);

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithHeader(string key, string value);

        /// <summary>
        /// Add a request header matcher
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithHeader(string key, IMatcher matcher);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithJsonBody(dynamic body);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithJsonBody(dynamic body, string contentType);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithJsonBody(dynamic body, JsonSerializerOptions settings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithJsonBody(dynamic body, JsonSerializerOptions settings, string contentType);

        /// <summary>
        /// A pre-formatted body which should be used as-is for the request 
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV4 WithBody(string body, string contentType);

        // TODO: Support binary and multi-part body

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        IResponseBuilderV4 WillRespond();
    }
}
