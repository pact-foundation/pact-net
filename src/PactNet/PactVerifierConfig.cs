using System.Collections.Generic;
using PactNet.Infrastructure.Outputters;

namespace PactNet
{
    /// <summary>
    /// Pact verifier configuration
    /// </summary>
    public class PactVerifierConfig
    {
        /// <summary>
        /// Log outputs
        /// </summary>
        public IEnumerable<IOutput> Outputters { get; set; } = new List<IOutput>
        {
            new ConsoleOutput()
        };

        public void WriteLine(string line)
        {
            foreach (IOutput output in this.Outputters)
            {
                output.WriteLine(line);
            }
        }
    }
}
