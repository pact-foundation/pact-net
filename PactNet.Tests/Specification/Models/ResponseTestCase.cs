using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Tests.Specification.Models
{
    public class ResponseTestCase : IVerifiable
    {
        private readonly IPactProviderServiceResponseComparer _responseComparer;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public PactProviderServiceResponse Expected { get; set; }
        public PactProviderServiceResponse Actual { get; set; }

        public ResponseTestCase()
        {
            _responseComparer = new PactProviderServiceResponseComparer();
        }

        public bool Verified()
        {
            try
            {
                _responseComparer.Compare(Expected, Actual);
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