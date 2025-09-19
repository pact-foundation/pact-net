using System;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous HTTP interactions
    /// </summary>
    internal class HttpInteractionDriver : IHttpInteractionDriver
    {
        private readonly PactHandle pact;
        private readonly InteractionHandle interaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="HttpInteractionDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="interaction">Interaction handle</param>
        internal HttpInteractionDriver(PactHandle pact, InteractionHandle interaction)
        {
            this.pact = pact;
            this.interaction = interaction;
        }

        /// <summary>
        /// Add a provider state to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <returns>Success</returns>
        public void Given(string description)
            => PactInterop.Given(this.interaction, description).ThrowExceptionOnFailure();

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Success</returns>
        public void GivenWithParam(string description, string name, string value)
            => PactInterop.GivenWithParam(this.interaction, description, name, value).ThrowExceptionOnFailure();

        /// <summary>
        /// Add a request to the interaction
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        public void WithRequest(string method, string path)
            => HttpInterop.WithRequest(this.interaction, method, path).ThrowExceptionOnFailure();

        /// <summary>
        /// Add a query string parameter to the interaction
        /// </summary>
        /// <param name="name">Query string parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="index">Parameter index (for if the same name is used multiple times)</param>
        public void WithQueryParameter(string name, string value, uint index)
            => HttpInterop.WithQueryParameter(this.interaction, name, new UIntPtr(index), value).ThrowExceptionOnFailure();

        /// <summary>
        /// Set a request header
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">Header value</param>
        /// <param name="index">Header index (for if the same header is added multiple times)</param>
        public void WithRequestHeader(string name, string value, uint index)
            => HttpInterop.WithHeader(this.interaction, InteractionPart.Request, name, new UIntPtr(index), value).ThrowExceptionOnFailure();

        /// <summary>
        /// Set a response header
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">Header value</param>
        /// <param name="index">Header index (for if the same header is added multiple times)</param>
        public void WithResponseHeader(string name, string value, uint index)
            => HttpInterop.WithHeader(this.interaction, InteractionPart.Response, name, new UIntPtr(index), value).ThrowExceptionOnFailure();

        /// <summary>
        /// Set the response status
        /// </summary>
        /// <param name="status">Status code</param>
        public void WithResponseStatus(ushort status)
            => HttpInterop.ResponseStatus(this.interaction, status).ThrowExceptionOnFailure();

        /// <summary>
        /// Set the request body
        /// </summary>
        /// <param name="contentType">Context type</param>
        /// <param name="body">Serialised body</param>
        public void WithRequestBody(string contentType, string body)
            => HttpInterop.WithBody(this.interaction, InteractionPart.Request, contentType, body).ThrowExceptionOnFailure();

        /// <summary>
        /// Set the response body
        /// </summary>
        /// <param name="contentType">Context type</param>
        /// <param name="body">Serialised body</param>
        public void WithResponseBody(string contentType, string body)
            => HttpInterop.WithBody(this.interaction, InteractionPart.Response, contentType, body).ThrowExceptionOnFailure();
    }
}
