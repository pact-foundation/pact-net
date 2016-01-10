using System;

namespace PactNet.Reporters
{
    internal class ConsoleReportOutputter : IReportOutputter
    {
        public void Write(string report)
        {
            Console.WriteLine(report);
        }
    }
}