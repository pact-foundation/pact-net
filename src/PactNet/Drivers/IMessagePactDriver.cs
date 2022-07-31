namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for message pacts
    /// </summary>
    internal interface IMessagePactDriver
    {
        /// <summary>
        /// Create a new message interaction on the current pact
        /// </summary>
        /// <param name="description">Interaction description</param>
        /// <returns>Message interaction driver</returns>
        IMessageInteractionDriver NewMessageInteraction(string description);

        /// <summary>
        /// Add metadata to the message pact
        /// </summary>
        /// <param name="namespace">the namespace</param>
        /// <param name="name">the name of the parameter</param>
        /// <param name="value">the value of the parameter</param>
        void WithMessagePactMetadata(string @namespace, string name, string value);
    }
}
