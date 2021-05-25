using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace PactNet.Backend.Native
{
    /// <summary>
    /// Mock request builder
    /// </summary>
    public class NativeRequestBuilder : IRequestBuilder
    {
        private readonly InteractionHandle interaction;
        private readonly JsonSerializerSettings defaultSettings;
        private readonly Dictionary<string, uint> queryCounts;
        private readonly Dictionary<string, uint> headerCounts;

        private bool requestConfigured = false;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativeRequestBuilder"/> class.
        /// </summary>
        /// <param name="interaction"></param>
        /// <param name="defaultSettings">Default JSON serializer settings</param>
        internal NativeRequestBuilder(InteractionHandle interaction, JsonSerializerSettings defaultSettings)
        {
            this.interaction = interaction;
            this.defaultSettings = defaultSettings;
            this.queryCounts = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
            this.headerCounts = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Add a provider state
        /// </summary>
        /// <param name="providerState">Provider state description</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder Given(string providerState)
        {
            MockServerInterop.Given(this.interaction, providerState);
            return this;
        }

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithRequest(HttpMethod method, string path) => this.WithRequest(method.Method, path);

        /// <summary>
        /// Set the request
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithRequest(string method, string path)
        {
            this.requestConfigured = true;

            MockServerInterop.WithRequest(this.interaction, method, path);
            return this;
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
            uint index = this.queryCounts.ContainsKey(key) ? this.queryCounts[key] + 1 : 0;
            this.queryCounts[key] = index;

            MockServerInterop.WithQueryParameter(this.interaction, key, new UIntPtr(index), value);
            return this;
        }

        /// <summary>
        /// Add a request header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithHeader(string key, string value)
        {
            uint index = this.headerCounts.ContainsKey(key) ? this.headerCounts[key] + 1 : 0;
            this.headerCounts[key] = index;

            MockServerInterop.WithHeader(this.interaction, InteractionPart.Request, key, new UIntPtr(index), value);
            return this;
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithJsonBody(dynamic body) => WithJsonBody(body, this.defaultSettings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        public IRequestBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            string serialised = JsonConvert.SerializeObject(body, settings);

            MockServerInterop.WithBody(this.interaction, InteractionPart.Request, "application/json", serialised);
            return this;
        }

        /// <summary>
        /// Define the response to this request
        /// </summary>
        /// <returns>Response builder</returns>
        public IResponseBuilder WillRespond()
        {
            if (!this.requestConfigured)
            {
                throw new InvalidOperationException("You must configure the request before defining the response");
            }

            var builder = new NativeResponseBuilder(this.interaction, this.defaultSettings);
            return builder;
        }
    }
}