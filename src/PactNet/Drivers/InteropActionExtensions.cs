using System.Runtime.InteropServices;
using PactNet.Exceptions;

namespace PactNet.Drivers
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
        public static void CheckInteropSuccess(this bool success)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Some FFI calls return false on MacOS, even when they succeed. See issue https://github.com/pact-foundation/pact-reference/issues/210
                return;
            }

            if (!success)
            {
                throw new PactFailureException("Unable to perform the given action. The interop call indicated failure");
            }
        }
    }
}
