namespace PactNet.Validators
{
    public interface IPactProviderResponseValidator
    {
        void Validate(PactProviderResponse expectedResponse, PactProviderResponse actualResponse);
    }
}