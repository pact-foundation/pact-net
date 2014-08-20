using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public interface IProviderServiceRequestComparer : IComparer<ProviderServiceRequest>
    {
        IReporter GetReporter();
    }
}
