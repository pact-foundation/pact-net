using PactNet.Infrastructure.Outputters;

namespace PactNet.Infrastructure.Outputters
{
    internal interface IOutputBuilder : IOutput
    {
        string Output { get; }
        void Clear();
    }
}