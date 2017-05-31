using PactNet.Mocks.MockHttpService.Models;
using Xunit;

namespace PactNet.Tests.IntegrationTests.Specification.Models
{
    //TODO: Rethink if we need this test case
    public class ResponseTestCase : IVerifiable
    {
        //private readonly IProviderServiceResponseComparer _responseComparer;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public ProviderServiceResponse Expected { get; set; }
        public ProviderServiceResponse Actual { get; set; }

        public ResponseTestCase()
        {
            //_responseComparer = new ProviderServiceResponseComparer();
        }

        public void Verify()
        {
            //var result = _responseComparer.Compare(Expected, Actual);

            if (Match)
            {
                //Assert.False(result.HasFailure, "There should not be any errors");
            }
            else
            {
                //Assert.Equal(1, result.Failures.Count());
            }
        }
    }
}