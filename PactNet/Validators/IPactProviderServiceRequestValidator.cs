namespace PactNet.Validators
{
    public interface IPactProviderServiceRequestValidator
    {
        void Validate(PactProviderServiceRequest expectedRequest, PactProviderServiceRequest actualRequest);
    }
}
