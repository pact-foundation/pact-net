using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace PactNet.Backend.Native
{
    /// <summary>
    /// Mock response builder
    /// </summary>
    public class NativeResponseBuilder : IResponseBuilder
    {
        private readonly InteractionHandle interaction;
        private readonly JsonSerializerSettings defaultSettings;
        private readonly Dictionary<string, uint> headerCounts;

        /// <summary>
        /// Initialises a new instance of the <see cref="NativeResponseBuilder"/> class.
        /// </summary>
        /// <param name="interaction">Interaction handle</param>
        /// <param name="defaultSettings">Default JSON serializer settings</param>
        internal NativeResponseBuilder(InteractionHandle interaction, JsonSerializerSettings defaultSettings)
        {
            this.interaction = interaction;
            this.defaultSettings = defaultSettings;
            this.headerCounts = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithStatus(HttpStatusCode status)
        {
            ushort converted = (ushort)status;

            MockServerInterop.ResponseStatus(this.interaction, converted);
            return this;
        }

        /// <summary>
        /// Set response status code
        /// </summary>
        /// <param name="status">Response status code</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithStatus(ushort status)
        {
            MockServerInterop.ResponseStatus(this.interaction, status);
            return this;
        }

        /// <summary>
        /// Add a response header
        /// </summary>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithHeader(string key, string value)
        {
            uint index = this.headerCounts.ContainsKey(key) ? this.headerCounts[key] + 1 : 0;
            this.headerCounts[key] = index;

            MockServerInterop.WithHeader(this.interaction, InteractionPart.Response, key, new UIntPtr(index), value);
            return this;
        }

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithJsonBody(dynamic body) => WithJsonBody(body, this.defaultSettings);

        /// <summary>
        /// Set a body which is serialised as JSON
        /// </summary>
        /// <param name="body">Request body</param>
        /// <param name="settings">Custom JSON serializer settings</param>
        /// <returns>Fluent builder</returns>
        public IResponseBuilder WithJsonBody(dynamic body, JsonSerializerSettings settings)
        {
            string serialised = JsonConvert.SerializeObject(body, settings);

            MockServerInterop.WithBody(this.interaction, InteractionPart.Response, "application/json", serialised);
            return this;
        }
    }
}