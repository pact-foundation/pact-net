using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace PactNet.Backend.Remote
{
    /// <summary>
    /// Remote mock request builder
    /// </summary>
    public class RemoteRequestBuilder : IRequestBuilder
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder Given(string providerState)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithRequest(HttpMethod method, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithRequest(string method, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="value">Query parameter value</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        public IRequestBuilder WithQuery(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithJsonBody(dynamic body)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        public IResponseBuilder WillRespond()
        {
            throw new NotImplementedException();
        }
    }
}