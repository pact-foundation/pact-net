using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public interface IMockContextService
    {
        PactProviderServiceRequest GetExpectedRequest();
        PactProviderServiceResponse GetExpectedResponse();
    }
}