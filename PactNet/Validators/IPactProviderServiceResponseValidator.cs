namespace PactNet.Validators
{
    public interface IPactProviderServiceResponseValidator
    {
        void Validate(PactProviderServiceResponse expectedResponse, PactProviderServiceResponse actualResponse);
    }
}