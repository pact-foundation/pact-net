using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;

namespace PactNet.Tests.Specification.Models
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