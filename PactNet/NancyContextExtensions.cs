using Nancy;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet
{
    public static class NancyContextExtensions
    {
        const string MockRequest = "MockRequest";
        const string MockResponse = "MockResponse";

        public static PactProviderServiceRequest GetMockRequest(this NancyContext context)
        {
            return context.Items.ContainsKey(MockRequest)
                ? context.Items[MockRequest] as PactProviderServiceRequest
                : null;
        }

        public static PactProviderServiceResponse GetMockResponse(this NancyContext context)
        {
            return context.Items.ContainsKey(MockResponse)
                    ? context.Items[MockResponse] as PactProviderServiceResponse 
                    : null;
        }

        public static void SetMockRequest(this NancyContext context, PactProviderServiceRequest mockRequest)
        {
            context.Items[MockRequest] = mockRequest;
        }

        public static void SetMockResponse(this NancyContext context, PactProviderServiceResponse mockResponse)
        {
            context.Items[MockResponse] = mockResponse;
        }
    }
}
