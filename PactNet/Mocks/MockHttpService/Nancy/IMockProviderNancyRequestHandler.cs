using Nancy;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal interface IMockProviderNancyRequestHandler
    {
        Response Handle(NancyContext context);
    }
}