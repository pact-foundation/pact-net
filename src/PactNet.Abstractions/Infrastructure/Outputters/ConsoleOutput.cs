using System;

namespace PactNet.Infrastructure.Outputters
{
    /// <summary>
    /// Console output
    /// </summary>
    public class ConsoleOutput : IOutput
    {
        /// <summary>
        /// Write a line to the console
        /// </summary>
        /// <param name="line">Line</param>
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
