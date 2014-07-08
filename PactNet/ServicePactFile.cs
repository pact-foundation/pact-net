using System.Collections.Generic;

namespace PactNet
{
    public class ServicePactFile : PactFile
    {
        //TODO: Maybe order in a predictable way, so that the file doesn't always change in git?
        public IEnumerable<PactServiceInteraction> Interactions { get; set; }
    }
}