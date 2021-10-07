namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Provider of the backend verification process
    /// </summary>
    internal interface IVerifierProvider
    {
        /// <summary>
        /// Verify the pact from the given args
        /// </summary>
        /// <param name="args">Verifier args</param>
        /// <returns>Verifier result</returns>
        PactVerifierResult Verify(string args);

        /// <summary>
        /// Get the logs for the current verification run
        /// </summary>
        /// <param name="provider">Name of the provider</param>
        /// <returns>Verifier logs</returns>
        string VerifierLogs(string provider);
    }
}
