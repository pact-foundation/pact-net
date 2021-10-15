using System;

namespace PactNet.Infrastructure.Outputters
{
    public class ConsoleOutput : IOutput
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}