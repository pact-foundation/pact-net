namespace PactNet.Comparers
{
    public interface IPactProviderServiceResponseComparer
    {
        void Compare(PactProviderServiceResponse response1, PactProviderServiceResponse response2);
    }
}