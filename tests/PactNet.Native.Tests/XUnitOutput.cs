using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace PactNet.Native.Tests
{
    public class XUnitOutput : IOutput
    {
        private readonly ITestOutputHelper output;

        public XUnitOutput(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void WriteLine(string line)
        {
            this.output.WriteLine(line);
        }
    }
}
