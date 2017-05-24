#if USE_KESTREL

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PactNet.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PactNet.Mocks.MockHttpService.Kestrel
{
    internal class KestrelHandler
    {
        private readonly RequestDelegate _next;
        private readonly IMockProviderRequestHandler _requestHandler;
        private readonly IMockProviderAdminRequestHandler _adminRequestHandler;
        private readonly ILog _log;
        private readonly PactConfig _pactConfig;

        public KestrelHandler(
            RequestDelegate next, IMockProviderRequestHandler requestHandler, IMockProviderAdminRequestHandler adminRequestHandler,
            ILog log, PactConfig pactConfig)
        {
            _next = next;
            _requestHandler = requestHandler;
            _adminRequestHandler = adminRequestHandler;
            _log = log;
            _pactConfig = pactConfig;
        }

        public async Task Invoke(HttpContext context)
        {
            IRequestWrapper request = await KestrelRequest.Create(context.Request);

            await _next(context);

            try
            {
                ResponseWrapper responseWrapper = IsAdminRequest(context.Request)
                    ? _adminRequestHandler.Handle(request)
                    : _requestHandler.Handle(request);

                context.Response.OnStarting(
                    state =>
                    {
                        foreach (KeyValuePair<string, string> header in responseWrapper.Headers)
                        {
                            ((HttpContext)state).Response.Headers.Add(header.Key, header.Value);
                        }

                        return Task.CompletedTask;
                    }, context);

                context.Response.StatusCode = (int)responseWrapper.StatusCode;
                await context.Response.Body.WriteAsync(responseWrapper.Contents, 0, responseWrapper.Contents.Length);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(PactFailureException))
                {
                    _log.ErrorException("Failed to handle the request", ex);
                }

                var exceptionMessage = $"{JsonConvert.ToString(ex.Message).Trim('"')} See {(!string.IsNullOrEmpty(_pactConfig.LoggerName) ? LogProvider.CurrentLogProvider.ResolveLogPath(_pactConfig.LoggerName) : "logs")} for details.";

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(exceptionMessage);
            }
        }

        private bool IsAdminRequest(HttpRequest request)
        {
            return request.Headers != null &&
                   request.Headers.Any(x => x.Key == Constants.AdministrativeRequestHeaderKey);
        }
    }
}

#endif