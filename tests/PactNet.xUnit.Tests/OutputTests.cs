using Moq;
using Xunit.Abstractions;

namespace PactNet.xUnit.Tests;

public class OutputTests
{
    [Fact]
    public void WriteLine()
    {
        var output = new Mock<ITestOutputHelper>();
        output
            .Setup(x => x.WriteLine("hello"))
            .Verifiable();

        var helper = output.Object.AsPactOutput();
        helper.WriteLine("hello");

        output.Verify();
    }
}
