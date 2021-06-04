namespace PactNet.Native
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
        void Verify(string args);
    }
}
