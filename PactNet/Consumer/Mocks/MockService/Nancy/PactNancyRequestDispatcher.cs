using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Routing;

namespace PactNet.Consumer.Mocks.MockService.Nancy
{
    public class PactNancyRequestDispatcher : IRequestDispatcher
    {
        private static PactProviderRequest _request;
        private static PactProviderResponse _response;

        public Task<Response> Dispatch(NancyContext context, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Response>();

            try
            {
                var response = HandleRequest(context.Request);
                context.Response = response;
                tcs.SetResult(context.Response);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        public static void Set(PactProviderRequest request)
        {
            _request = request;
        }

        public static void Set(PactProviderResponse response)
        {
            _response = response;
        }

        private Response HandleRequest(Request request)
        {
            this.FilterRequest(request);
            return this.GenerateResponse();
        }

        private Response GenerateResponse()
        {
            var mapper = new NancyResponseMapper();
            return mapper.Convert(_response);
        }

        private void FilterRequest(Request request)
        {
            if (_request == null)
                throw new Exception("Pact Request not set");

            // TODO: Handle in a better way and return mismatches
            this.CompareHeaders(request);
            this.CompareBody(request);
        }

        private void CompareHeaders(Request request)
        {
            foreach (var providerHeader in _request.Headers)
            {
                // Check header exists in Nancy Headers
                var nancyHeader = request.Headers.FirstOrDefault(header => header.Key == providerHeader.Key);

                // If matching nancy header doesn't exist return false
                if (!nancyHeader.Value.Contains(providerHeader.Value))
                    throw new Exception("Pact Request Header Not Found in Nancy Request: " + providerHeader.Key + " " + providerHeader.Value);
            }
        }

        private void CompareBody(Request request)
        {
            using (var reader = new StreamReader(request.Body, Encoding.UTF8))
            {
                var nancyBody = reader.ReadToEnd();
                var pactBody = _request.Body != null ? Newtonsoft.Json.JsonConvert.SerializeObject(_request.Body) : string.Empty;

                if (!nancyBody.Equals(pactBody, StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Nancy Pact Request Body Mismatch");
            }
        }

        public static void Reset()
        {
            _request = null;
            _response = null;
        }
    }
}