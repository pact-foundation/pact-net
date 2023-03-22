using PactNet.Infrastructure.Outputters;
using Xunit.Abstractions;

namespace PactNet.xUnit;

public static class Factory
{
    public static IOutput AsPactOutput(this ITestOutputHelper output) => new Output(output);
}
