using PactNet.Models;

namespace PactNet.Validators
{
    public interface IPactValidator<in TPactFile> where TPactFile : PactFile
    {
        void Validate(TPactFile pactFile);
    }
}
