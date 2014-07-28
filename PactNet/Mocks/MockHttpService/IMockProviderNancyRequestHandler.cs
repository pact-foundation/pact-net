using Nancy;

namespace PactNet.Mocks.MockHttpService
{
    public interface IMockProviderNancyRequestHandler
    {
        Response Handle(NancyContext context);
    }
}