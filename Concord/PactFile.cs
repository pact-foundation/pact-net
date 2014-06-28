using System.Collections.Generic;

namespace Concord
{
    public class PactFile
    {
        public PactParty Provider { get; set; }
        public PactParty Consumer { get; set; }
        public IEnumerable<PactInteraction> Interactions { get; set; }
        public dynamic Metadata { get; set; }

        public void VerifyProvider()
        {
            //TODO: Send requests and ensure the provider responds as document in the pact
        }
    }
}