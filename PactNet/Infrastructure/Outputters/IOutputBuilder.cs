using PactNet.Infrastructure.Outputters;

namespace PactNet.Infrastructure.Outputters
{
    internal interface IOutputBuilder : IOutput
    {
        void Clear();
    }
}