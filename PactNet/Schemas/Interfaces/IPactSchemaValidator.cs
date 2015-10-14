using Newtonsoft.Json.Linq;
using PactNet.Models;

namespace PactNet.Schemas.Interfaces
{
    internal interface IPactSchemaValidator<in TPactFile> where TPactFile : PactFile
    {
        void Validate(TPactFile pactFile, ProviderStates providerStates, JObject jsonObjToValidate);
    }
}
