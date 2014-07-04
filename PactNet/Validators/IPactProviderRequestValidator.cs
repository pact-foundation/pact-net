using Nancy;

namespace PactNet.Validators
{
    public interface IPactProviderRequestValidator
    {
        void Validate(PactProviderRequest expectedRequest, Request actualRequest);
    }
}
