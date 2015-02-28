using System;

namespace PactNet.Reporters
{
    public class ConsoleReportOutputter : IReportOutputter
    {
        private Indent SetupCorrectIndentation(int tabDepth)
        {
            var indent = new Indent(0);
            for (var i = 0; i < tabDepth; i++)
            {
                indent.Increment();
            }
            return indent;
        }

        public void WriteInfo(string infoMessage, int tabDepth)
        {
            var indent = SetupCorrectIndentation(tabDepth);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(indent + infoMessage);
            Console.ResetColor();
        }

        public void WriteError(string errorMessage, int tabDepth)
        {
            var indent = SetupCorrectIndentation(tabDepth);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(indent + errorMessage);
            Console.ResetColor();
        }

        public void WriteSuccess(string successMessage, int tabDepth = 0)
        {
            var indent = SetupCorrectIndentation(tabDepth);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(indent + successMessage);
            Console.ResetColor();
        }
    }
}