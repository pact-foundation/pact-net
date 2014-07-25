using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public interface IMockProviderService : IMockProvider<IMockProviderService>
    {
        string BaseUri { get; }
        IMockProviderService With(PactProviderServiceRequest request);
        void WillRespondWith(PactProviderServiceResponse response);
        void Start();
        void Stop();
        void ClearTestScopedInteractions();
    }
}