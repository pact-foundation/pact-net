namespace PactNet
{
    /// <summary>
    /// Message pact v3 Builder
    /// </summary>
    public interface IMessagePactBuilderV3
    {
        /// <summary>
        /// Add a new message to the pact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 ExpectsToReceive(string description);

        /// <summary>
        /// Add metadata information to message pact
        /// </summary>
        /// <param name="namespace">the parent configuration section</param>
        /// <param name="name">the metadata field value</param>
        /// <param name="value">the metadata field value</param>
        /// <returns>Fluent builder</returns>
        IMessagePactBuilderV3 WithPactMetadata(string @namespace, string name, string value);
    }

    /// <summary>
    /// Message pact v4 Builder
    /// </summary>
    public interface IMessagePactBuilderV4
    {
        /// <summary>
        /// Add a new message to the pact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV4 ExpectsToReceive(string description);

        /// <summary>
        /// Add metadata information to message pact
        /// </summary>
        /// <param name="namespace">the parent configuration section</param>
        /// <param name="name">the metadata field value</param>
        /// <param name="value">the metadata field value</param>
        /// <returns>Fluent builder</returns>
        IMessagePactBuilderV4 WithPactMetadata(string @namespace, string name, string value);
    }
}
