using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public interface IHttpRequestSender
    {
        ProviderServiceResponse Send(ProviderServiceRequest request);
    }
}