using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using PactNet.Models.Consumer.Dsl;

namespace PactNet.Comparers
{
    internal interface IDslPartComparer
    {
        ComparisonResult Compare(DslPart expected, JToken actual);
    }
}
