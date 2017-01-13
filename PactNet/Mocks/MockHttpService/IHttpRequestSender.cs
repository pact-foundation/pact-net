using System.Threading.Tasks;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    internal interface IHttpRequestSender
    {
        Task<ProviderServiceResponse> Send(ProviderServiceRequest request);
    }
}