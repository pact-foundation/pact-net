using System;
using System.Net;
using Newtonsoft.Json;

namespace PactNet.Remote
{
    /// <summary>
    /// Remote mock response builder
    /// </summary>
    public class RemoteResponseBuilder : IResponseBuilderV2, IResponseBuilderV3
    {
        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithStatus(HttpStatusCode status)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithStatus(ushort status)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithJsonBody(dynamic body)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithStatus(ushort status)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithJsonBody(dynamic body)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a response body which is serialised as JSON
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithStatus(HttpStatusCode status)
        {
            throw new NotImplementedException();
        }
    }
}
