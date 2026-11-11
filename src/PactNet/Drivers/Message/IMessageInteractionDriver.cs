namespace PactNet.Drivers.Message
{
    /// <summary>
    /// Driver for asynchronous message interactions
    /// </summary>
    internal interface IMessageInteractionDriver : IProviderStateDriver, ICompletedPactDriver
    {
        /// <summary>
        /// Set the description of the message interaction
        /// </summary>
        /// <param name="description">message description</param>
        void ExpectsToReceive(string description);

        /// <summary>
        /// Set the metadata of the message
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        void WithMetadata(string key, string value);

        /// <summary>
        /// Set the contents of the message
        /// </summary>
        /// <param name="contentType">the content type</param>
        /// <param name="body">the body of the message</param>
        /// <param name="size">the size of the message</param>
        void WithContents(string contentType, string body, uint size);

        /// <summary>
        /// Returns the message without the matchers
        /// </summary>
        /// <returns>Reified message</returns>
        string Reify();
    }
}
