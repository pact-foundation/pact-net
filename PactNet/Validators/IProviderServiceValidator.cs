namespace PactNet.Validators
{
    public interface IProviderServiceValidator
    {
        void Validate(ServicePactFile pactFile);
    }
}