namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous (request/response) message interactions
    /// </summary>
    internal interface ISyncMessageInteractionDriver : IProviderStateDriver, ICompletedPactDriver
    {
        /// <summary>
        /// Add a reference to an external resource, e.g. an AsyncAPI operation, to the interaction
        /// </summary>
        /// <param name="group">Reference group, e.g. the name of the external specification</param>
        /// <param name="name">Reference name, e.g. operationId</param>
        /// <param name="value">Reference value</param>
        void AddReference(string group, string name, string value);

        /// <summary>
        /// Set a metadata value of the request message
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value as JSON, which may contain matchers</param>
        void WithRequestMetadata(string key, string value);

        /// <summary>
        /// Set a metadata value of all the existing response messages
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value as JSON, which may contain matchers</param>
        void WithResponseMetadata(string key, string value);

        /// <summary>
        /// Set the contents of the request message
        /// </summary>
        /// <param name="contentType">the content type</param>
        /// <param name="body">the body of the request message</param>
        void WithRequestContents(string contentType, string body);

        /// <summary>
        /// Set the contents of the response message
        /// </summary>
        /// <param name="contentType">the content type</param>
        /// <param name="body">the body of the response message</param>
        void WithResponseContents(string contentType, string body);

        /// <summary>
        /// Get the actual request and response contents, with any matchers removed and any
        /// configured generators applied
        /// </summary>
        /// <returns>The generated request and response contents</returns>
        string GenerateContents();

        /// <summary>
        /// Match actual contents against an expected response, applying the response's matching rules
        /// </summary>
        /// <param name="index">Index of the expected response</param>
        /// <param name="contentType">Content type of the actual contents</param>
        /// <param name="contents">Actual contents</param>
        /// <returns>JSON array of mismatches, which is empty if the contents matched</returns>
        string MatchResponseContents(int index, string contentType, string contents);
    }
}
