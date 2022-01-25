using System.Collections.Generic;
using PactNet.Infrastructure.Outputters;

namespace PactNet.Verifier
{
    /// <summary>
    /// Pact verifier configuration
    /// </summary>
    public class PactVerifierConfig
    {
        /// <summary>
        /// Log level for the verifier
        /// </summary>
        public PactLogLevel LogLevel { get; set; } = PactLogLevel.Information;

        /// <summary>
        /// Log outputs
        /// </summary>
        public IEnumerable<IOutput> Outputters { get; set; } = new List<IOutput>
        {
            new ConsoleOutput()
        };

        /// <summary>
        /// Write a line to every configured output
        /// </summary>
        /// <param name="line">Line to write</param>
        public void WriteLine(string line)
        {
            foreach (IOutput output in this.Outputters)
            {
                output.WriteLine(line);
            }
        }
    }
}
