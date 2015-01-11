using System.Linq;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.IntegrationTests.Specification.Models
{
    public class RequestTestCase : IVerifiable
    {
        private readonly IProviderServiceRequestComparer _requestComparer;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public ProviderServiceRequest Expected { get; set; }
        public ProviderServiceRequest Actual { get; set; }

        public RequestTestCase()
        {
            _requestComparer = new ProviderServiceRequestComparer();
        }

        public void Verify()
        {
            var result = _requestComparer.Compare(Expected, Actual);

            if (Match)
            {
                Assert.Empty(result.Errors);
                foreach (var childResult in result.ComparisonResults)
                {
                    Assert.Empty(childResult.Errors);
                }
            }
            else
            {
                Assert.Equal(1, result.Errors.Count() + result.ComparisonResults.Sum(childResult => childResult.Errors.Count()));
            }
        }
    }
}