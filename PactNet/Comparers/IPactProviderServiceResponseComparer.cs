namespace PactNet.Comparers
{
    public interface IPactProviderServiceResponseComparer
    {
        void Compare(PactProviderServiceResponse expectedResponse, PactProviderServiceResponse actualResponse);
    }
}