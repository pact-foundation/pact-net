using PactNet.Infrastructure.Outputters;

namespace PactNet.Mocks.MockAmqpService.Host
{
    internal interface IWaitForResultOutputter : IOutput
    {
        string FullOutput { get; }
        void Clear();
    }
}