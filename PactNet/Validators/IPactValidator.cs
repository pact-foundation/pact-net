using PactNet.Models;

namespace PactNet.Validators
{
    internal interface IPactValidator<in TPactFile> where TPactFile : PactFile
    {
        void Validate(TPactFile pactFile, ProviderStates providerStates);
        void Validate(TPactFile pactFile);
    }
}
