using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PactNet.Mocks.MockHttpService.Matchers
{
    public interface IJValueMatcher
    {
        bool Match(JValue expected, JToken actual);
    }
}
