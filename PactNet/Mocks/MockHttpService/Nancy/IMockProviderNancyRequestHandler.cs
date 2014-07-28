using Nancy;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public interface IMockProviderNancyRequestHandler
    {
        Response Handle(NancyContext context);
    }
}