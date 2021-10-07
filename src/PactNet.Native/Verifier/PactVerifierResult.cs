namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Pact verification result
    /// </summary>
    internal enum PactVerifierResult
    {
        /// <summary>
        /// An unknown error has occurred
        /// </summary>
        UnknownError = -1,

        /// <summary>
        /// Pact verifier succeeded
        /// </summary>
        Success,

        /// <summary>
        /// Pact verification failed
        /// </summary>
        Failure,

        /// <summary>
        /// A null pointer was passed to the verifier
        /// </summary>
        NullPointer,

        /// <summary>
        /// The verifier panicked
        /// </summary>
        Panic,

        /// <summary>
        /// Invalid arguments were provided to the verifier
        /// </summary>
        InvalidArguments,
    }
}
