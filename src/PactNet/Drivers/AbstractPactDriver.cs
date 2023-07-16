using System;
using PactNet.Interop;

namespace PactNet.Drivers
{
    /// <summary>
    /// Abstract pact driver, agnostic of interaction style
    /// </summary>
    internal abstract class AbstractPactDriver : ICompletedPactDriver
    {
        private readonly PactHandle pact;

        /// <summary>
        /// Initialises a new instance of the <see cref="AbstractPactDriver"/> class.
        /// </summary>
        /// <param name="pact">Pact handle</param>
        protected AbstractPactDriver(PactHandle pact)
        {
            this.pact = pact;
        }

        /// <summary>
        /// Write the pact file to disk
        /// </summary>
        /// <param name="directory">Directory of the pact file</param>
        /// <returns>Status code</returns>
        /// <exception cref="InvalidOperationException">Failed to write pact file</exception>
        public void WritePactFile(string directory)
        {
            var result = NativeInterop.WritePactFile(this.pact, NativeInterop.StringToUtf8(directory), false);

            if (result != 0)
            {
                throw result switch
                {
                    1 => new InvalidOperationException("The pact reference library panicked"),
                    2 => new InvalidOperationException("The pact file could not be written"),
                    3 => new InvalidOperationException("A mock server with the provided port was not found"),
                    _ => new InvalidOperationException($"Unknown error from backend: {result}")
                };
            }
        }
    }
}
