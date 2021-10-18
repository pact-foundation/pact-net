using System.Collections.Generic;
using Provider.Tests;

namespace PactNet.AspNetCore.ProviderState
{
    public class ProviderStateInteraction
    {
        public string State { get; set; }

        public StateAction Action { get; set; }

        public IDictionary<string, string> Params { get; set; }
    }
}
