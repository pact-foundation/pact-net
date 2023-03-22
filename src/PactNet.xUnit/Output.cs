using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace PactNet.xUnit;

internal class Output : IOutput
{
    private readonly ITestOutputHelper output;

    public Output(ITestOutputHelper output) => this.output = output;

    public void WriteLine(string line) => this.output.WriteLine(line);
}
