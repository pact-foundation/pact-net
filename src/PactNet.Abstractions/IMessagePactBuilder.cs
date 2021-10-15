using System;
using System.Threading.Tasks;

namespace PactNet
{
    /// <summary>
    /// Message pact Builder
    /// </summary>
    public interface IMessagePactBuilder
    {
        /// <summary>
        /// Verify a message is read and handled correctly
        /// </summary>
        /// <param name="handler">The method using the message</param>
        void Verify<T>(Action<T> handler);

        /// <summary>
        /// Verify a message is read and handled correctly
        /// </summary>
        /// <param name="handler">The method using the message</param>
        Task VerifyAsync<T>(Func<T, Task> handler);
    }

    /// <summary>
    /// Message pact v3 Builder
    /// </summary>
    public interface IMessagePactBuilderV3 : IMessagePactBuilder
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
}
