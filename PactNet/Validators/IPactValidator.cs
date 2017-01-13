using System.Threading.Tasks;
using PactNet.Models;

namespace PactNet.Validators
{
    internal interface IPactValidator<in TPactFile> where TPactFile : PactFile
    {
        Task Validate(TPactFile pactFile, ProviderStates providerStates);
    }
}
