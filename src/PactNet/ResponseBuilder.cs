using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using PactNet.Drivers;
using PactNet.Matchers;

namespace PactNet
{
    /// <summary>
    /// Mock response builder
    /// </summary>
    internal class ResponseBuilder : IResponseBuilderV2, IResponseBuilderV3
    {
        private readonly IHttpInteractionDriver driver;
        private readonly JsonSerializerSettings defaultSettings;
        private readonly Dictionary<string, uint> headerCounts;

        /// <summary>
        /// Initialises a new instance of the <see cref="ResponseBuilder"/> class.
        /// </summary>
        /// <param name="driver">Interaction driver</param>
        /// <param name="defaultSettings">Default JSON serializer settings</param>
        internal ResponseBuilder(IHttpInteractionDriver driver, JsonSerializerSettings defaultSettings)
        {
            this.driver = driver;
            this.defaultSettings = defaultSettings;
            this.headerCounts = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
        }

        #region IResponseBuilderV2 explicit implementation

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithStatus(HttpStatusCode status)
            => this.WithStatus(status);

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithStatus(ushort status)
            => this.WithStatus(status);

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithHeader(string key, string value)
            => this.WithHeader(key, value);

        /// <summary>
        /// Add a response header matcher
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithHeader(string key, IMatcher matcher)
            => this.WithHeader(key, matcher);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithJsonBody(dynamic body)
            => this.WithJsonBody(body);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithJsonBody(dynamic body, JsonSerializerSettings settings)
            => this.WithJsonBody(body, settings);

        /// <summary>
        /// A pre-formatted body which should be used as-is for the response 
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV2 IResponseBuilderV2.WithBody(string body, string contentType)
            => this.WithBody(body, contentType);

        #endregion

        #region IResponseBuilderV3 explicit implementation

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithStatus(HttpStatusCode status)
            => this.WithStatus(status);

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithStatus(ushort status)
            => this.WithStatus(status);

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithHeader(string key, string value)
            => this.WithHeader(key, value);

        /// <summary>
        /// Add a response header matcher
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithHeader(string key, IMatcher matcher)
            => this.WithHeader(key, matcher);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithJsonBody(dynamic body)
            => this.WithJsonBody(body);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithJsonBody(dynamic body, JsonSerializerSettings settings)
            => this.WithJsonBody(body, settings);

        /// <summary>
        /// A pre-formatted body which should be used as-is for the response 
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        IResponseBuilderV3 IResponseBuilderV3.WithBody(string body, string contentType)
            => this.WithBody(body, contentType);

        #endregion

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        internal ResponseBuilder WithStatus(HttpStatusCode status)
        {
            ushort converted = (ushort)status;

            this.driver.WithResponseStatus(converted);
            return this;
        }

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        internal ResponseBuilder WithStatus(ushort status)
        {
            this.driver.WithResponseStatus(status);
            return this;
        }

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        internal ResponseBuilder WithHeader(string key, string value)
        {
            uint index = this.headerCounts.ContainsKey(key) ? this.headerCounts[key] + 1 : 0;
            this.headerCounts[key] = index;

            this.driver.WithResponseHeader(key, value, index);

            return this;
        }

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="matcher">Header value matcher</param>
        /// <returns>Fluent builder</returns>
        internal ResponseBuilder WithHeader(string key, IMatcher matcher)
        {
            var serialised = JsonConvert.SerializeObject(matcher, this.defaultSettings);

            return this.WithHeader(key, serialised);
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        internal ResponseBuilder WithJsonBody(dynamic body)
            => this.WithJsonBody(body, this.defaultSettings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        internal ResponseBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            string serialised = JsonConvert.SerializeObject(body, settings);
            return this.WithBody(serialised, "application/json");
        }

        /// <summary>
        /// A pre-formatted body which should be used as-is for the response 
        /// </summary>
        /// <param name="body">Response body</param>
        /// <param name="contentType">Content type</param>
        /// <returns>Fluent builder</returns>
        internal ResponseBuilder WithBody(string body, string contentType)
        {
            this.driver.WithResponseBody(contentType, body);
            return this;
        }
    }
}
