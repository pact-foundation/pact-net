using System;

namespace PactNet.Exceptions
{
    /// <summary>
    /// Error relating to a failing pact test
    /// </summary>
    [Serializable]
    public class PactFailureException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PactFailureException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public PactFailureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="PactFailureException" /> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified</param>
        public PactFailureException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
