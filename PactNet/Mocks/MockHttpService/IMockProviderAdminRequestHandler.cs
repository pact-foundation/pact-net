namespace PactNet.Mocks.MockHttpService
{
    internal interface IMockProviderAdminRequestHandler
    {
        ResponseWrapper Handle(IRequestWrapper request);
    }
}