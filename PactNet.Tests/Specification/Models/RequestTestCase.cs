using PactNet.Comparers;
using PactNet.Validators;

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
                _requestComparer.Validate(Expected, Actual);
            }
            catch (PactComparisonFailed)
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