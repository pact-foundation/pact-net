using System;
using System.Collections.Generic;
using System.Text;
using PactNet.Verifier.ProviderState;

namespace PactNet.Exceptions
{
    /// <summary>
    /// Error related to verifying a message pact consumer test
    /// </summary>
    [Serializable]
    public class StateHandlerConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateHandlerExecutionException" /> class
        /// </summary>
        public StateHandlerConfigurationException(StateHandler stateHandler)
            : this($"State handler \"{stateHandler.Description}\" already added at \"{stateHandler.Action}\"")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateHandlerExecutionException" /> class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public StateHandlerConfigurationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateHandlerExecutionException" /> class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified</param>
        public StateHandlerConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
