using Nancy;

namespace Concord
{
    public class PactServiceNancyModule : NancyModule
    {
        private static PactServiceRequest _request;
        private static PactServiceResponse _response;

        public PactServiceNancyModule()
        {
            if (_request.Method == HttpVerb.Get)
            {
                Get[_request.Path/*, ctx => ctx.Request.Equals(_request)*/] = _ =>
                {
                    var mapper = new NancyResponseMapper();
                    var response = mapper.Convert(_response);

                    return response;
                };
            }
        }

        public static void Set(PactServiceRequest request, PactServiceResponse response)
        {
            Reset();

            _request = request;
            _response = response;
        }

        private static void Reset()
        {
            _request = null;
            _response = null;
        }
    }
}