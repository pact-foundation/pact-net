namespace Concord
{
    public class PactService
    {
        private string _description;
        private PactServiceRequest _request;
        private PactServiceResponse _response;

        public PactService(int port)
        {
            //Server should be booted here
        }

        public PactService UponReceiving(string description)
        {
            _description = description;

            return this;
        }

        public PactService With(PactServiceRequest request)
        {
            _request = request;

            return this;
        }

        public PactService WillResponseWith(PactServiceResponse response)
        {
            _response = response;

            return this;
        }
    }
}
