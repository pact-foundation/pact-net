using System;
using PactNet.Comparers;
using PactNet.Mocks.MockHttpService.Validators;

namespace PactNet.Mocks.MockHttpService.Comparers
{
    public class HttpStatusCodeComparer : IHttpStatusCodeComparer
    {
        public ComparisonResult Compare(int expected, int actual)
        {
            var result = new ComparisonResult();

            var indent = new Indent(5);

            result.AddInfo(String.Format("{0}has status code {1}", indent, expected));

            if (!expected.Equals(actual))
            {
                result.AddError(expected: expected, actual: actual);
            }

            return result;
        }
    }
}