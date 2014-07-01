using Nancy;

namespace Concord
{
    public class PactProviderNancyModule : NancyModule
    {
        private static PactProviderRequest _request;
        private static PactProviderResponse _response;

        public PactProviderNancyModule()
        {
            if (_request == null || _response == null)
            {
                return;
            }
            
            switch (_request.Method)
            {
                case HttpVerb.Head:
                case HttpVerb.Get:
                    Get[_request.Path, ctx => FilterRequest(ctx.Request, _request)] = _ => GenerateResponse();
                    break;
                case HttpVerb.Post:
                    Post[_request.Path, ctx => FilterRequest(ctx.Request, _request)] = _ => GenerateResponse();
                    break;
                case HttpVerb.Put:
                    Put[_request.Path, ctx => FilterRequest(ctx.Request, _request)] = _ => GenerateResponse();
                    break;
                case HttpVerb.Delete:
                    Delete[_request.Path, ctx => FilterRequest(ctx.Request, _request)] = _ => GenerateResponse();
                    break;
                case HttpVerb.Patch:
                    Patch[_request.Path, ctx => FilterRequest(ctx.Request, _request)] = _ => GenerateResponse();
                    break;
            }
        }

        public static void Set(PactProviderRequest request, PactProviderResponse response)
        {
            Reset();

            _request = request;
            _response = response;
        }

        private Response GenerateResponse()
        {
            var mapper = new NancyResponseMapper();
            return mapper.Convert(_response);
        }

        private bool FilterRequest(Request nancyRequest, PactProviderRequest providerRequest)
        {
            //Compare headers
            //Compare Body
            return true; //Just for now, don't filter any requests
        }

        private static void Reset()
        {
            _request = null;
            _response = null;
        }
    }
}