using System;

namespace PactNet.Reporters.Outputters
{
    internal class ConsoleReportOutputter : IReportOutputter
    {
        public void Write(string report)
        {
            Console.WriteLine(report);
        }
    }
}