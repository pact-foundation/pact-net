using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Tests.Specification.Models
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

        public bool Verified()
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
                }
                return true;
            }

            if (!Match)
            {
                return false;
            }

            return true;
        }
    }
}