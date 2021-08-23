using PactNet.Native.Interop;

namespace PactNet.Native
{
    /// <summary>
    /// Mock server
    /// </summary>
    internal interface IMessageMockServer
    {
        /// <summary>
        /// Set the messagePact specification version
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="version">Specification version</param>
        /// <returns>Success</returns>
        bool WithSpecification(PactHandle pact, PactSpecification version);

        /// <summary>
        /// Write the messagePact message file
        /// </summary>
        /// <param name="pact">the messagePact</param>
        /// <param name="directory">the output folder</param>
        /// <param name="overwrite">overwrite</param>
        /// <returns></returns>
        void WriteMessagePactFile(MessagePactHandle pact, string directory, bool overwrite);

        /// <summary>
        /// Add metadata to the message messagePact
        /// </summary>
        /// <param name="pact">the messagePact message handle</param>
        /// <param name="namespace">the namespace</param>
        /// <param name="name">the name of the parameter</param>
        /// <param name="value">the value of the parameter</param>
        /// <returns>Success</returns>
        bool WithMessagePactMetadata(MessagePactHandle pact, string @namespace, string name, string value);

        /// <summary>
        /// Create a new message messagePact
        /// </summary>
        /// <param name="consumerName">Consumer name</param>
        /// <param name="providerName">Provider name</param>
        /// <returns>Pact handle</returns>
        MessagePactHandle NewMessagePact(string consumerName, string providerName);

        /// <summary>
        /// Create a new message on the given messagePact
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="description">Interaction description</param>
        /// <returns></returns>
        public MessageHandle NewMessage(MessagePactHandle pact, string description);

        /// <summary>
        /// Set the description of the message description
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="description">message description</param>
        /// <returns>Success</returns>
        bool ExpectsToReceive(MessageHandle message, string description);

        /// <summary>
        /// Add a provider state to the message
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="description">Provider state description</param>
        /// <returns>Success</returns>
        bool Given(MessageHandle message, string description);

        /// <summary>
        /// Add a provider state with a parameter to the interaction
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="description">Provider state description</param>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Success</returns>
        bool GivenWithParam(MessageHandle message, string description, string name, string value);

        /// <summary>
        /// Set the metadata of the message
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        /// <returns>Success</returns>
        bool WithMetadata(MessageHandle message, string key, string value);

        /// <summary>
        /// Set the contents of the message
        /// </summary>
        /// <param name="message">message</param>
        /// <param name="contentType">the content type</param>
        /// <param name="body">the body of the message</param>
        /// <param name="size">the size of the message</param>
        /// <returns>Success</returns>
        bool WithContents(MessageHandle message, string contentType, string body, uint size);

        /// <summary>
        /// returns the message without the matchers
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>Success</returns>
        string Reify(MessageHandle message);
    }
}
