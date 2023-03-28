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
            => NativeInterop.Given(this.interaction, description).CheckInteropSuccess();

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Success</returns>
        public void GivenWithParam(string description, string name, string value)
            => NativeInterop.GivenWithParam(this.interaction, description, name, value).CheckInteropSuccess();

        /// <summary>
        /// Add a request to the interaction
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        public void WithRequest(string method, string path)
            => NativeInterop.WithRequest(this.interaction, method, path).CheckInteropSuccess();

        /// <summary>
        /// Add a query string parameter to the interaction
        /// </summary>
        /// <param name="name">Query string parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="index">Parameter index (for if the same name is used multiple times)</param>
        public void WithQueryParameter(string name, string value, uint index)
            => NativeInterop.WithQueryParameter(this.interaction, name, new UIntPtr(index), value).CheckInteropSuccess();

        /// <summary>
        /// Set a request header
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">Header value</param>
        /// <param name="index">Header index (for if the same header is added multiple times)</param>
        public void WithRequestHeader(string name, string value, uint index)
            => NativeInterop.WithHeader(this.interaction, InteractionPart.Request, name, new UIntPtr(index), value).CheckInteropSuccess();

        /// <summary>
        /// Set a response header
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">Header value</param>
        /// <param name="index">Header index (for if the same header is added multiple times)</param>
        public void WithResponseHeader(string name, string value, uint index)
            => NativeInterop.WithHeader(this.interaction, InteractionPart.Response, name, new UIntPtr(index), value).CheckInteropSuccess();

        /// <summary>
        /// Set the response status
        /// </summary>
        /// <param name="status">Status code</param>
        public void WithResponseStatus(ushort status)
            => NativeInterop.ResponseStatus(this.interaction, status).CheckInteropSuccess();

        /// <summary>
        /// Set the request body
        /// </summary>
        /// <param name="contentType">Context type</param>
        /// <param name="body">Serialised body</param>
        public void WithRequestBody(string contentType, string body)
            => NativeInterop.WithBody(this.interaction, InteractionPart.Request, contentType, body).CheckInteropSuccess();

        /// <summary>
        /// Set the response body
        /// </summary>
        /// <param name="contentType">Context type</param>
        /// <param name="body">Serialised body</param>
        public void WithResponseBody(string contentType, string body)
            => NativeInterop.WithBody(this.interaction, InteractionPart.Response, contentType, body).CheckInteropSuccess();

        /// <summary>
        /// Set the request body to multipart/form-data for file upload
        /// </summary>
        /// <param name="contentType">Content type override</param>
        /// <param name="filePath">path to file being uploaded</param>
        /// <param name="partName">the name of the mime part being uploaded</param>
        public void WithFileUpload(string contentType, string filePath, string partName)
            => NativeInterop.WithFileUpload(this.interaction, InteractionPart.Request, contentType, filePath, partName).CheckInteropSuccess();
    }
}
