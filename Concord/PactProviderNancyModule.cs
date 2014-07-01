namespace Concord
{
    using Nancy;
    using System;

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
                    Get[_request.Path] = _ => HandleRequest();
                    break;
                case HttpVerb.Post:
                    Post[_request.Path] = _ => HandleRequest();
                    break;
                case HttpVerb.Put:
                    Put[_request.Path] = _ => HandleRequest();
                    break;
                case HttpVerb.Delete:
                    Delete[_request.Path] = _ => HandleRequest();
                    break;
                case HttpVerb.Patch:
                    Patch[_request.Path] = _ => HandleRequest();
                    break;
            }
        }

        public static void Set(PactProviderRequest request, PactProviderResponse response)
        {
            _request = request;
            _response = response;
        }

        private Response HandleRequest()
        {
            // TODO: Handle this in a better way
            if (!this.FilterRequest(this.Context.Request, _request))
                throw new Exception("Nancy Pact Request Mismatch");

            return this.GenerateResponse();
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