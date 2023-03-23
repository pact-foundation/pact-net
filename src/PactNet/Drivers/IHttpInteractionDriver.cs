namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous HTTP interactions
    /// </summary>
    internal interface IHttpInteractionDriver : IProviderStateDriver
    {
        /// <summary>
        /// Add a request to the interaction
        /// </summary>
        /// <param name="method">Request method</param>
        /// <param name="path">Request path</param>
        void WithRequest(string method, string path);

        /// <summary>
        /// Add a query string parameter to the interaction
        /// </summary>
        /// <param name="name">Query string parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="index">Parameter index (for if the same name is used multiple times)</param>
        void WithQueryParameter(string name, string value, uint index);

        /// <summary>
        /// Set a request header
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">Header value</param>
        /// <param name="index">Header index (for if the same header is added multiple times)</param>
        void WithRequestHeader(string name, string value, uint index);

        /// <summary>
        /// Set a response header
        /// </summary>
        /// <param name="name">header name</param>
        /// <param name="value">Header value</param>
        /// <param name="index">Header index (for if the same header is added multiple times)</param>
        void WithResponseHeader(string name, string value, uint index);

        /// <summary>
        /// Set the response status
        /// </summary>
        /// <param name="status">Status code</param>
        void WithResponseStatus(ushort status);

        /// <summary>
        /// Set the request body
        /// </summary>
        /// <param name="contentType">Context type</param>
        /// <param name="body">Serialised body</param>
        void WithRequestBody(string contentType, string body);

        /// <summary>
        /// Set the response body
        /// </summary>
        /// <param name="contentType">Context type</param>
        /// <param name="body">Serialised body</param>
        void WithResponseBody(string contentType, string body);


        /// <summary>
        /// Set the response body for a single file to be uploaded as a multipart/form-data content type
        /// </summary>
        /// <param name="filePath">path to file being uploaded</param>
        /// <param name="contentType">Content type override</param>
        /// <param name="mimePartName">the name of the mime part being uploaded</param>
        void WithMultipartSingleFileUpload(string filePath, string contentType, string mimePartName);
    }
}
