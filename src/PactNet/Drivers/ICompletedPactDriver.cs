using System;

namespace PactNet.Drivers
{
    /// <summary>
    /// Driver for writing completed pact files containing interactions
    /// </summary>
    public interface ICompletedPactDriver
    {
        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="directory">Directory of the pact file</param>
        /// <returns>Status code</returns>
        /// <exception cref="InvalidOperationException">Failed to write pact file</exception>
        void WritePactFile(string directory);
    }
}
