namespace PactNet.Validators
{
    public interface IPactProviderRequestValidator
    {
        void Validate(PactProviderRequest expectedRequest, PactProviderRequest actualRequest);
    }
}
