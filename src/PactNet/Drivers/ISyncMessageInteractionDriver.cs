namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for synchronous (request/response) message interactions
    /// </summary>
    internal interface ISyncMessageInteractionDriver : IProviderStateDriver, ICompletedPactDriver
    {
        /// <summary>
        /// Set the metadata of the request message
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        void WithRequestMetadata(string key, string value);

        /// <summary>
        /// Set the metadata of the response message
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
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
        /// Returns the request and response contents without the matchers, with any configured
        /// generators applied
        /// </summary>
        /// <returns>Reified message</returns>
        string Reify();
    }
}
