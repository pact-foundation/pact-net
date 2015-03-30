using System;
using System.Text;

namespace PactNet.Reporters
{
    internal class ConsoleReportOutputter : IReportOutputter
    {
        private const string AnIndent = "  ";

        public void WriteInfo(string infoMessage, int tabDepth = 0)
        {
            var indentation = GetIndentation(tabDepth);

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(indentation + infoMessage);
            Console.ResetColor();
        }

        public void WriteError(string errorMessage, int tabDepth = 0)
        {
            var indentation = GetIndentation(tabDepth);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(indentation + errorMessage);
            Console.ResetColor();
        }

        public void WriteSuccess(string successMessage, int tabDepth = 0)
        {
            var indentation = GetIndentation(tabDepth);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(indentation + successMessage);
            Console.ResetColor();
        }

        private static string GetIndentation(int tabDepth)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < tabDepth; i++)
            {
                builder.Append(AnIndent);
            }
            return builder.ToString();
        }
    }
}