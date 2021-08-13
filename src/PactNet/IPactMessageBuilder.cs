using System;

namespace PactNet
{
    public interface IPactMessageBuilder
    {
        /// <summary>
        /// Verify a message is read and handled correctly
        /// </summary>
        /// <param name="handler">The method using the message</param>
        void Verify<T>(Action<T> handler);
    }

    /// <summary>
    /// Build up a mock message for a v3 pact
    /// </summary>
    public interface IPactMessageBuilderV3 : IPactMessageBuilder
    {
        /// <summary>
        /// Add a new message to the pact
        /// </summary>
        /// <param name="description">Message description</param>
        /// <returns>Fluent builder</returns>
        IMessageBuilderV3 ExpectsToReceive(string description);
    }
}
