using System.Text;

namespace PactNet.Infrastructure.Outputters
{
    public class OutputBuilder : IOutputBuilder
    {
        private readonly StringBuilder _outputBuilder = new StringBuilder();

        public void WriteLine(string line)
        {
            _outputBuilder.AppendLine(line);
        }

        public string Output => _outputBuilder.ToString();

        public void Clear()
        {
            _outputBuilder.Clear();
        }
    }
}
