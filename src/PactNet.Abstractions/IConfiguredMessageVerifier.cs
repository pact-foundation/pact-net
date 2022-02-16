using System;
using System.Threading.Tasks;

namespace PactNet
{
    /// <summary>
    /// A configured message type state, which can now be verified
    /// </summary>
    public interface IConfiguredMessageVerifier
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
}
