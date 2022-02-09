using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Matchers;

namespace PactNet.Remote
{
    /// <summary>
    /// Remote mock request builder
    /// </summary>
    public class RemoteRequestBuilder : IRequestBuilderV2, IRequestBuilderV3
    {
        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.Given(string providerState)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithRequest(HttpMethod method, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithRequest(string method, string path)
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
        IRequestBuilderV2 IRequestBuilderV2.WithQuery(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithHeader(string key, IMatcher matcher)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithJsonBody(dynamic body)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        IResponseBuilderV2 IRequestBuilderV2.WillRespond()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.Given(string providerState)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Flient builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.Given(string providerState, IDictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithRequest(HttpMethod method, string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithRequest(string method, string path)
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
        IRequestBuilderV3 IRequestBuilderV3.WithQuery(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithHeader(string key, IMatcher matcher)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithJsonBody(dynamic body)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        IResponseBuilderV3 IRequestBuilderV3.WillRespond()
        {
            throw new NotImplementedException();
        }
    }
}
