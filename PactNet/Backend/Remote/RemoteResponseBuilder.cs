using System;
using System.Net;
using Newtonsoft.Json;

namespace PactNet.Backend.Remote
{
    /// <summary>
    /// Remote mock response builder
    /// </summary>
    public class RemoteResponseBuilder : IResponseBuilder
    {
        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithStatus(HttpStatusCode status)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithStatus(ushort status)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithJsonBody(dynamic body)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}