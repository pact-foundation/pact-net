using System;
using System.IO;
using System.Linq;
using System.Text;
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
            this.FilterRequest();
            return this.GenerateResponse();
        }

        private Response GenerateResponse()
        {
            var mapper = new NancyResponseMapper();
            return mapper.Convert(_response);
        }

        private void FilterRequest()
        {
            if (_request == null)
                throw new Exception("Pact Request not set");

            // TODO: Handle in a better way and return mismatches
            this.CompareHeaders();
            this.CompareBody();
        }

        private void CompareHeaders()
        {
            foreach (var providerHeader in _request.Headers)
            {
                // Check header exists in Nancy Headers
                var nancyHeader = this.Request.Headers.FirstOrDefault(header => header.Key == providerHeader.Key);

                // If matching nancy header doesn't exist return false
                if (!nancyHeader.Value.Contains(providerHeader.Value))
                    throw new Exception("Nancy Pact Request Header Mismatch");
            }
        }

        private void CompareBody()
        {
            using (var reader = new StreamReader(this.Request.Body, Encoding.UTF8))
            {
                var nancyBody = reader.ReadToEnd();
                var pactBody = _request.Body != null ? Newtonsoft.Json.JsonConvert.SerializeObject(_request.Body) : string.Empty;

                if (!nancyBody.Equals(pactBody, StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Nancy Pact Request Body Mismatch");
            }
        }

        private static void Reset()
        {
            _request = null;
            _response = null;
        }
    }
}