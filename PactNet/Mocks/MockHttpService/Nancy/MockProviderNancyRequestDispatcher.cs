using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nancy;
using Nancy.Routing;
using Newtonsoft.Json;
using PactNet.Logging;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    internal class MockProviderNancyRequestDispatcher : IRequestDispatcher
    {
        private readonly IMockProviderRequestHandler _requestHandler;
        private readonly IMockProviderAdminRequestHandler _adminRequestHandler;
        private readonly ILog _log;
        private readonly PactConfig _pactConfig;

        public MockProviderNancyRequestDispatcher(
            IMockProviderRequestHandler requestHandler,
            IMockProviderAdminRequestHandler adminRequestHandler,
            ILog log,
            PactConfig pactConfig)
        {
            _requestHandler = requestHandler;
            _adminRequestHandler = adminRequestHandler;
            _log = log;
            _pactConfig = pactConfig;
        }

        public Task<Response> Dispatch(NancyContext context, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<Response>();

            if (cancellationToken.IsCancellationRequested)
            {
                tcs.SetException(new OperationCanceledException());
                return tcs.Task;
            }

            if (context == null)
            {
                tcs.SetException(new ArgumentException("context is null"));
                return tcs.Task;
            }

            Response response;

            try
            {
                response = IsAdminRequest(context.Request) ?
                    _adminRequestHandler.Handle(context) :
                    _requestHandler.Handle(context);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(PactFailureException))
                {
                    _log.ErrorException("Failed to handle the request", ex);
                }

                var exceptionMessage =
                    $"{JsonConvert.ToString(ex.Message).Trim('"')} See {(!string.IsNullOrEmpty(_pactConfig.LoggerName) ? LogProvider.CurrentLogProvider.ResolveLogPath(_pactConfig.LoggerName) : "logs")} for details.";

                response = new Response
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = exceptionMessage,
                    Contents = s =>
                    {
                        var bytes = Encoding.UTF8.GetBytes(exceptionMessage);
                        s.Write(bytes, 0, bytes.Length);
                        s.Flush();
                    }
                };
            }

            context.Response = response;
            tcs.SetResult(context.Response);

            return tcs.Task;
        }

        private static bool IsAdminRequest(Request request)
        {
            return request.Headers != null &&
                   request.Headers.Any(x => x.Key == Constants.AdministrativeRequestHeaderKey);
        }
    }
}