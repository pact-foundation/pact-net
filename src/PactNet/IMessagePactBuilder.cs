using System;
using System.Threading.Tasks;

namespace PactNet
{
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
        Task VerifyAsync<T>(Action<T> handler);
    }

    /// <summary>
    /// Build up a mock message for a v3 messagePact
    /// </summary>
    public interface IMessagePactBuilderV3 : IMessagePactBuilder
    {
        /// <summary>
        /// Add a new message to the messagePact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 ExpectsToReceive(string description);
    }
}
