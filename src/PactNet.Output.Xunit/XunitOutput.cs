using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace PactNet.Output.Xunit
{
    /// <summary>
    /// Output to an xUnit test output helper
    /// </summary>
    public class XunitOutput : IOutput
    {
        private readonly ITestOutputHelper output;

        /// <summary>
        /// Initialises a new instance of the <see cref="XunitOutput"/> class.
        /// </summary>
        /// <param name="output">xUnit test output helper</param>
        public XunitOutput(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// Write a line to the output
        /// </summary>
        /// <param name="line">Line to write</param>
        public void WriteLine(string line) => this.output.WriteLine(line);
    }
}
