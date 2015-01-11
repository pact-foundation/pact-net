using System.Linq;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.IntegrationTests.Specification.Models
{
    public class ResponseTestCase : IVerifiable
    {
        private readonly IProviderServiceResponseComparer _responseComparer;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public ProviderServiceResponse Expected { get; set; }
        public ProviderServiceResponse Actual { get; set; }

        public ResponseTestCase()
        {
            _responseComparer = new ProviderServiceResponseComparer();
        }

        public void Verify()
        {
            var result = _responseComparer.Compare(Expected, Actual);

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