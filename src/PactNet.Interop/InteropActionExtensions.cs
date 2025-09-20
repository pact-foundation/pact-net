using PactNet.Exceptions;

namespace PactNet.Interop
{
    /// <summary>
    /// Extensions for checking interop action success
    /// </summary>
    internal static class InteropActionExtensions
    {
        /// <summary>
        /// Check the result of an interop action to ensure it succeeded
        /// </summary>
        /// <param name="success">Action succeeded</param>
        /// <exception cref="PactFailureException">Action failed</exception>
        public static void ThrowExceptionOnFailure(this bool success)
        {
            if (!success)
            {
                throw new PactFailureException("Unable to perform the given action. The interop call indicated failure");
            }
        }
    }
}
