using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using PactNet.Generators;
using PactNet.Interop;
using PactNet.Matchers;

namespace PactNet
{
    /// <summary>
    /// Mock request builder
    /// </summary>
    internal class RequestBuilder : IRequestBuilderV2, IRequestBuilderV3
    {
        private readonly IMockServer server;
        private readonly InteractionHandle interaction;
        private readonly JsonSerializerSettings defaultSettings;
        private readonly Dictionary<string, uint> queryCounts;
        private readonly Dictionary<string, uint> headerCounts;

        private bool requestConfigured = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="RequestBuilder"/> class.
        /// </summary>
        /// <param name="server">Mock server</param>
        /// <param name="interaction"></param>
        /// <param name="defaultSettings">Default JSON serializer settings</param>
        internal RequestBuilder(IMockServer server, InteractionHandle interaction, JsonSerializerSettings defaultSettings)
        {
            this.server = server;
            this.interaction = interaction;
            this.defaultSettings = defaultSettings;
            this.queryCounts = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
            this.headerCounts = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
        }

        #region IRequestBuilderV2 explicit implementation

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.Given(string providerState)
            => this.Given(providerState);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithRequest(HttpMethod method, string path)
            => this.WithRequest(method, path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithRequest(string method, string path)
            => this.WithRequest(method, path);

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="value">Query parameter value</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilderV2 IRequestBuilderV2.WithQuery(string key, string value)
            => this.WithQuery(key, value);

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithHeader(string key, string value)
            => this.WithHeader(key, value);

        /// <summary>
        /// Add a request header matcher
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithHeader(string key, IMatcher matcher)
            => this.WithHeader(key, matcher);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithJsonBody(dynamic body)
            => this.WithJsonBody(body);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithJsonBody(dynamic body, string contentType)
            => this.WithJsonBody(body, contentType);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithJsonBody(dynamic body, JsonSerializerSettings settings)
            => this.WithJsonBody(body, settings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithJsonBody(dynamic body, JsonSerializerSettings settings, string contentType)
            => this.WithJsonBody(body, settings, contentType);

        /// <summary>
        /// A pre-formatted body which should be used as-is for the request 
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV2 IRequestBuilderV2.WithBody(string body, string contentType)
            => this.WithBody(body, contentType);

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        IResponseBuilderV2 IRequestBuilderV2.WillRespond()
            => this.WillRespond();

        #endregion

        #region IRequestBuilderV3 explicit implementation

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.Given(string providerState)
            => this.Given(providerState);

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Flient builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.Given(string providerState, IDictionary<string, string> parameters)
            => this.Given(providerState, parameters);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithRequest(HttpMethod method, string path)
            => this.WithRequest(method, path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="generator">Path value generator</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithRequest(HttpMethod method, IGenerator generator)
            => this.WithRequest(method, generator);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithRequest(string method, string path)
            => this.WithRequest(method, path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="generator">Path value generator</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithRequest(string method, IGenerator generator)
            => this.WithRequest(method, generator);

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="value">Query parameter value</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilderV3 IRequestBuilderV3.WithQuery(string key, string value)
            => this.WithQuery(key, value);

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="generator">Query parameter value generator</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        IRequestBuilderV3 IRequestBuilderV3.WithQuery(string key, IGenerator generator)
            => this.WithQuery(key, generator);

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithHeader(string key, string value)
            => this.WithHeader(key, value);

        /// <summary>
        /// Add a request header matcher
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithHeader(string key, IMatcher matcher)
            => this.WithHeader(key, matcher);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithJsonBody(dynamic body)
            => this.WithJsonBody(body);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithJsonBody(dynamic body, string contentType)
            => this.WithJsonBody(body, contentType);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithJsonBody(dynamic body, JsonSerializerSettings settings)
            => this.WithJsonBody(body, settings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithJsonBody(dynamic body, JsonSerializerSettings settings, string contentType)
            => this.WithJsonBody(body, settings, contentType);

        /// <summary>
        /// A pre-formatted body which should be used as-is for the request 
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        IRequestBuilderV3 IRequestBuilderV3.WithBody(string body, string contentType)
            => this.WithBody(body, contentType);

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        IResponseBuilderV3 IRequestBuilderV3.WillRespond()
            => this.WillRespond();

        #endregion

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder Given(string providerState)
        {
            this.server.Given(this.interaction, providerState);
            return this;
        }

        /// <summary>
        /// Add a provider state with one or more parameters
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <param name="parameters">Provider state parameters</param>
        /// <returns>Flient builder</returns>
        internal RequestBuilder Given(string providerState, IDictionary<string, string> parameters)
        {
            foreach (var param in parameters)
            {
                this.server.GivenWithParam(this.interaction, providerState, param.Key, param.Value);
            }

            return this;
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithRequest(HttpMethod method, string path)
            => this.WithRequest(method.Method, path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="generator">Path value generator</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithRequest(HttpMethod method, IGenerator generator)
            => this.WithRequest(method.Method, generator);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithRequest(string method, string path)
        {
            this.requestConfigured = true;

            this.server.WithRequest(this.interaction, method, path);
            return this;
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="generator">Path value generator</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithRequest(string method, IGenerator generator)
        {
            var serialised = JsonConvert.SerializeObject(generator, this.defaultSettings);
            return this.WithRequest(method, serialised);
        }

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="value">Query parameter value</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        internal RequestBuilder WithQuery(string key, string value)
        {
            uint index = this.queryCounts.ContainsKey(key) ? this.queryCounts[key] + 1 : 0;
            this.queryCounts[key] = index;

            this.server.WithQueryParameter(this.interaction, key, value, index);
            return this;
        }

        /// <summary>
        /// Add a query string parameter
        /// </summary>
        /// <param name="key">Query parameter key</param>
        /// <param name="generator">Query parameter value generator</param>
        /// <returns>Fluent builder</returns>
        /// <remarks>You can add a query parameter with the same key multiple times</remarks>
        internal RequestBuilder WithQuery(string key, IGenerator generator)
        {
            var serialised = JsonConvert.SerializeObject(generator, this.defaultSettings);
            return this.WithQuery(key, serialised);
        }


        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithHeader(string key, string value)
        {
            uint index = this.headerCounts.ContainsKey(key) ? this.headerCounts[key] + 1 : 0;
            this.headerCounts[key] = index;

            this.server.WithRequestHeader(this.interaction, key, value, index);

            return this;
        }

        /// <summary>
        /// Add a request header matcher
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithHeader(string key, IMatcher matcher)
        {
            var serialised = JsonConvert.SerializeObject(matcher, this.defaultSettings);

            return this.WithHeader(key, serialised);
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithJsonBody(dynamic body) => WithJsonBody(body, this.defaultSettings, "application/json");

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithJsonBody(dynamic body, string contentType) => WithJsonBody(body, this.defaultSettings, contentType);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings) => WithJsonBody(body, settings, "application/json");

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <param name="contentType">Content type override</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings, string contentType)
        {
            string serialised = JsonConvert.SerializeObject(body, settings);
            return this.WithBody(serialised, contentType);
        }

        /// <summary>
        /// A pre-formatted body which should be used as-is for the request 
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        internal RequestBuilder WithBody(string body, string contentType)
        {
            this.server.WithRequestBody(this.interaction, contentType, body);
            return this;
        }

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        internal ResponseBuilder WillRespond()
        {
            if (!this.requestConfigured)
            {
                throw new InvalidOperationException("You must configure the request before defining the response");
            }

            var builder = new ResponseBuilder(this.server, this.interaction, this.defaultSettings);
            return builder;
        }
    }
}
