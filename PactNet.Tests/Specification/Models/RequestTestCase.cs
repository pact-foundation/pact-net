using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Tests.Specification.Models
{
    public class RequestTestCase
    {
        private readonly IPactProviderServiceRequestComparer _requestComparer;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public PactProviderServiceRequest Expected { get; set; }
        public PactProviderServiceRequest Actual { get; set; }

        public RequestTestCase()
        {
            _requestComparer = new PactProviderServiceRequestComparer();
        }

        public bool Verify()
        {
            try
            {
                _requestComparer.Compare(Expected, Actual);
            }
            catch (CompareFailedException)
            {
                if (Match)
                {
                    return false;
                    //throw;
                }
            }

            if (!Match)
            {
                return false;
                //throw new Exception("Test passed, but should have failed.");
            }

            return true;
        }
    }
}