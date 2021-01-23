using System.Collections.Generic;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Mocks.MockHttpService
{
    public interface IMockProviderService : IMockProvider<IMockProviderService>
    {
        IMockProviderService With(ProviderServiceRequest request);
        bool UseRemoteMockService { get; set; }
        void WillRespondWith(ProviderServiceResponse response);
        void Start();
        void Stop();
        void ClearInteractions();
        void VerifyInteractions();
        void SendAdminHttpRequest(HttpVerb method, string path, Dictionary<string, string> headers = null);
    }
}