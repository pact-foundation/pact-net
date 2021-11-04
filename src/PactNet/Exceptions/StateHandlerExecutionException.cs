using System;

namespace PactNet.Exceptions
{
    /// <summary>
    /// Error related to verifying a message pact consumer test
    /// </summary>
    [Serializable]
    public class StateHandlerExecutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateHandlerExecutionException" /> class
        /// </summary>
        public StateHandlerExecutionException()
            : this("The state handler has an action that doesn't match the provider state in the contract. Please compare your contract and your configured state handler in your provider tests.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateHandlerExecutionException" /> class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public StateHandlerExecutionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateHandlerExecutionException" /> class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified</param>
        public StateHandlerExecutionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
