using System;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Routing;
using PactNet.Validators;

namespace PactNet.Consumer.Mocks.MockService.Nancy
{
    public class PactNancyRequestDispatcher : IRequestDispatcher
    {
        private static PactProviderRequest _request;
        private static PactProviderResponse _response;
        private readonly IPactProviderRequestValidator _requestValidator;

        public PactNancyRequestDispatcher(IPactProviderRequestValidator requestValidator)
        {
            _requestValidator = requestValidator;
        }

        public Task<Response> Dispatch(NancyContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
            _requestValidator.Validate(_request, request);
            return GenerateResponse();
        }

        private Response GenerateResponse()
        {
            var mapper = new NancyResponseMapper();
            return mapper.Convert(_response);
        }

        public static void Reset()
        {
            _request = null;
            _response = null;
        }
    }
}