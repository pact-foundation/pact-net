using System;
using System.Threading.Tasks;

namespace PactNet
{
    /// <summary>
    /// A configured synchronous (request/response) message type state
    /// </summary>
    public interface IConfiguredSyncMessageVerifier
    {
        /// <summary>
        /// Verify a request message is read and handled correctly
        /// </summary>
        /// <param name="handler">The method handling the request message and producing a response</param>
        void Verify<TRequest, TResponse>(Func<TRequest, TResponse> handler);

        /// <summary>
        /// Verify a request message is read and handled correctly
        /// </summary>
        /// <param name="handler">The method handling the request message and producing a response</param>
        Task VerifyAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler);
    }
}
