using PactNet.Validators;

namespace PactNet.Tests.Specification.Models
{
    public class RequestTestCase
    {
        private readonly IPactProviderServiceRequestValidator _requestValidator;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public PactProviderServiceRequest Expected { get; set; }
        public PactProviderServiceRequest Actual { get; set; }

        public RequestTestCase()
        {
            _requestValidator = new PactProviderServiceRequestValidator();
        }

        public bool Verify()
        {
            try
            {
                _requestValidator.Validate(Expected, Actual);
            }
            catch (PactAssertException)
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