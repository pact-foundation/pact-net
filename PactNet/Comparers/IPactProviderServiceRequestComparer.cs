namespace PactNet.Comparers
{
    public interface IPactProviderServiceRequestComparer
    {
        void Validate(PactProviderServiceRequest request1, PactProviderServiceRequest request2);
    }
}
