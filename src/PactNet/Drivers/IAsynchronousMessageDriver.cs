using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for asynchronous message interactions
    /// </summary>
    internal interface IAsynchronousMessageDriver : IInteractionDriver, ICompletedPactDriver
    {
        /// <summary>
        /// Add metadata to the message message pact
        /// </summary>
        /// <param name="pact">the message pact message handle</param>
        /// <param name="namespace">the namespace</param>
        /// <param name="name">the name of the parameter</param>
        /// <param name="value">the value of the parameter</param>
        /// <returns>Success</returns>
        bool WithMessagePactMetadata(PactHandle pact, string @namespace, string name, string value);

        /// <summary>
        /// Set the description of the message interaction
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="description">message description</param>
        /// <returns>Success</returns>
        bool ExpectsToReceive(InteractionHandle message, string description);

        /// <summary>
        /// Set the metadata of the message
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        /// <returns>Success</returns>
        bool WithMetadata(InteractionHandle message, string key, string value);

        /// <summary>
        /// Set the contents of the message
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="contentType">the content type</param>
        /// <param name="body">the body of the message</param>
        /// <param name="size">the size of the message</param>
        /// <returns>Success</returns>
        bool WithContents(InteractionHandle message, string contentType, string body, uint size);

        /// <summary>
        /// Returns the message without the matchers
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>Success</returns>
        string Reify(InteractionHandle message);
    }
}
