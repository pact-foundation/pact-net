using System;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for writing completed pact files containing interactions
    /// </summary>
    internal interface ICompletedPactDriver
    {
        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="pact">Pact handle</param>
        /// <param name="directory">Directory of the pact file</param>
        /// <param name="overwrite">Overwrite the existing pact file?</param>
        /// <returns>Status code</returns>
        /// <exception cref="InvalidOperationException">Failed to write pact file</exception>
        void WritePactFile(PactHandle pact, string directory, bool overwrite);

        /// <summary>
        /// Get the driver logs
        /// </summary>
        /// <returns>Logs</returns>
        string Logs();
    }
}
