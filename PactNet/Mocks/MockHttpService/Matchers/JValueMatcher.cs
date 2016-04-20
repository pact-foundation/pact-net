using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace PactNet.Mocks.MockHttpService.Matchers
{
    public class JValueMatcher : IJValueMatcher
    {
        public bool Match(JValue expected, JToken actual)
        {
            return actual != null && expected.Equals(actual);
        }
    }
}
