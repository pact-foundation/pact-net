using PactNet.Models;

namespace PactNet.Mocks.MockHttpService.Models
{
    public class PactServiceInteraction : PactInteraction
    {
        public PactProviderServiceRequest Request { get; set; }
        public PactProviderServiceResponse Response { get; set; }
    }
}