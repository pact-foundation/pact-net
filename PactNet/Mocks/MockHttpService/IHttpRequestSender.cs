using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    internal interface IHttpRequestSender
    {
        ProviderServiceResponse Send(ProviderServiceRequest request);
    }
}