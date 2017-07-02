using System.Collections.Generic;
using PactNet.Infrastructure.Outputters;

namespace PactNet.Core
{
    internal interface IPactCoreHostConfig
    {
        string Path { get; }
        string Arguments { get; }
        bool WaitForExit { get; }
        IEnumerable<IOutput> Outputters { get; }
    }
}