using System.Runtime.InteropServices;
using PactNet.Exceptions;
using PactNet.Interop;

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
            if (!success)
            {
                throw new PactFailureException("Unable to perform the given action. The interop call indicated failure");
            }
        }

        /// <summary>
        /// Check the result of an interop action when the response is a StringResult
        /// </summary>
        /// <param name="success">The result of the action</param>
        /// <exception cref="PactFailureException">Action failed</exception>
        public static void CheckInteropSuccess(this StringResult success)
        {
            if (success.tag != StringResult.Tag.StringResult_Ok)
            {
                string errorMsg = Marshal.PtrToStringAnsi(success.failed.errorPointer);
                throw new PactFailureException($"Unable to perform the given action. The interop call returned failure: {errorMsg}");
            }
        }
    }
}
