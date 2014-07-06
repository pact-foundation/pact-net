using PactNet.Validators;

namespace PactNet.Tests.Specification.Models
{
    public class RequestTestCase
    {
        private readonly IPactProviderRequestValidator _requestValidator;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public PactProviderRequest Expected { get; set; }
        public PactProviderRequest Actual { get; set; }

        public RequestTestCase()
        {
            _requestValidator = new PactProviderRequestValidator();
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