using System.Text;

namespace PactNet.Mocks.MockAmqpService.Host
{
    public class WaitForResultOutputter : IWaitForResultOutputter
    {
        private readonly StringBuilder _outputBuilder = new StringBuilder();

        public void WriteLine(string line)
        {
            _outputBuilder.AppendLine(line);
        }

        public string FullOutput => _outputBuilder.ToString();

        public void Clear()
        {
            _outputBuilder.Clear();
        }
    }
}
