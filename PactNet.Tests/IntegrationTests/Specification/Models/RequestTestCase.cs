using System.Linq;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Models.ProviderService;
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
                Assert.False(result.HasFailure, "There should not be any errors");
            }
            else
            {
                Assert.Equal(1, result.Failures.Count());
            }
        }
    }
}