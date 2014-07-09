using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public interface IMockProviderService : IMockProvider<IMockProviderService>
    {
        IMockProviderService With(PactProviderServiceRequest request);
        IMockProviderService WillRespondWith(PactProviderServiceResponse response);
        void Start();
        void Stop();
    }
}