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
        /// Verify a request message is read and handled correctly, and that the response returned
        /// by the handler matches the configured response content
        /// </summary>
        /// <param name="handler">The method handling the request message and producing a response</param>
        /// <exception cref="Exceptions.PactMessageConsumerVerificationException">
        /// The handler threw, or its response did not match the configured response content
        /// </exception>
        void Verify<TRequest, TResponse>(Func<TRequest, TResponse> handler);

        /// <summary>
        /// Verify a request message is read and handled correctly, and that the response returned
        /// by the handler matches the configured response content
        /// </summary>
        /// <param name="handler">The method handling the request message and producing a response</param>
        /// <exception cref="Exceptions.PactMessageConsumerVerificationException">
        /// The handler threw, or its response did not match the configured response content
        /// </exception>
        Task VerifyAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler);
    }
}
