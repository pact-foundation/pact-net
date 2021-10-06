using PactNet.Exceptions;
using PactNet.Native.Interop;

namespace PactNet.Native.Verifier
{
    /// <summary>
    /// Native pact verifier
    /// </summary>
    internal class NativePactVerifier : IVerifierProvider
    {
        /// <summary>
        /// Verify the pact from the given args
        /// </summary>
        /// <param name="args">Verifier args</param>
        public void Verify(string args)
        {
            int result = NativeInterop.Verify(args);

            if (result == 0)
            {
                return;
            }

            throw result switch
            {
                1 => new PactFailureException("The verification process failed, see output for errors"),
                2 => new PactFailureException("A null pointer was received"),
                3 => new PactFailureException("The method panicked"),
                4 => new PactFailureException("Invalid arguments were provided to the verification process"),
                _ => new PactFailureException($"An unknown error occurred with error code {result}")
            };
        }
    }
}
