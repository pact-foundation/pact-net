using PactNet.Models.ProviderService;

namespace PactNet.Mocks.MockHttpService
{
    internal interface IHttpRequestSender
    {
        ProviderServiceResponse Send(ProviderServiceRequest request);
    }
}