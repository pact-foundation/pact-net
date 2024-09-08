using System;

namespace PactNet.Exceptions
{
    /// <summary>
    /// Pact verification failed
    /// </summary>
    [Serializable]
    public class PactVerificationFailedException : PactFailureException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PactVerificationFailedException" /> class
        /// </summary>
        public PactVerificationFailedException()
            : this("The pact failed verification")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PactVerificationFailedException" /> class
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public PactVerificationFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PactVerificationFailedException" /> class
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified</param>
        public PactVerificationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
