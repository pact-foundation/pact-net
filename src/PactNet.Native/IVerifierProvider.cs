namespace PactNet.Native
{
    /// <summary>
    /// Provider of the backend verification process
    /// </summary>
    internal interface IVerifierProvider
    {
        /// <summary>
        /// Verify the messagePact from the given args
        /// </summary>
        /// <param name="args">Verifier args</param>
        void Verify(string args);
    }
}
