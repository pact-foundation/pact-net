namespace PactNet.Mocks.MockHttpService
{
    internal interface IMockProviderRequestHandler
    {
        ResponseWrapper Handle(IRequestWrapper context);
    }
}