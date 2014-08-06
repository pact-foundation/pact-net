using System;

namespace PactNet.Reporters
{
    public class ConsoleReportOutputter : IReportOutputter
    {
        public void WriteInfo(string infoMessage, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(infoMessage, args);
            Console.ResetColor();
        }

        public void WriteError(string errorMessage, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage, args);
            Console.ResetColor();
        }
    }
}