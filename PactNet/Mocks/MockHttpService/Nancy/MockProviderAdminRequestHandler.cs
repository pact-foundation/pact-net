using System;
using System.IO;
using System.Linq;
using System.Text;
using Nancy;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class MockProviderAdminRequestHandler : IMockProviderAdminRequestHandler
    {
        private readonly IMockProviderRepository _mockProviderRepository;
        private readonly IProviderServiceRequestComparer _requestComparer;
        private readonly IReporter _reporter;

        public MockProviderAdminRequestHandler(
            IMockProviderRepository mockProviderRepository,
            IReporter reporter,
            IProviderServiceRequestComparer requestComparer)
        {
            _mockProviderRepository = mockProviderRepository;
            _reporter = reporter;
            _requestComparer = requestComparer;
        }

        public Response Handle(NancyContext context)
        {
            return HandleAdminRequest(context);
        }

        private Response HandleAdminRequest(NancyContext context)
        {
            if (context.Request.Method.Equals("DELETE", StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Path == "/interactions")
            {
                _mockProviderRepository.ClearHandledRequests();

                return GenerateResponse(HttpStatusCode.OK, "Successfully cleared the handled requests");
            }

            if (context.Request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Path == "/interactions/verification")
            {
                if (_mockProviderRepository == null)
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }

                if (_mockProviderRepository.HandledRequests != null && _mockProviderRepository.HandledRequests.Any())
                {
                    //Check number of calls

                    //Check actual request against matching request
                    foreach (var stat in _mockProviderRepository.HandledRequests)
                    {
                        _requestComparer.Compare(stat.MatchedInteraction.Request, stat.ActualRequest);
                    }
                }

                try
                {
                    _reporter.ThrowIfAnyErrors(); //TODO: Change this to return a http based error
                }
                catch (Exception)
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }

                return new Response
                {
                    StatusCode = HttpStatusCode.OK
                };
            }

            return GenerateResponse(HttpStatusCode.NotFound, 
                String.Format("The {0} request for path {1}, does not have a matching mock provider admin action.", context.Request.Method, context.Request.Path));
        }

        private Response GenerateResponse(HttpStatusCode statusCode, string message)
        {
            return new Response
            {
                StatusCode = statusCode,
                ReasonPhrase = message,
                Contents = s => SetContent(message, s)
            };
        }

        private void SetContent(string content, Stream stream)
        {
            var contentBytes = Encoding.UTF8.GetBytes(content);
            stream.Position = 0;
            stream.Write(contentBytes, 0, contentBytes.Length);
            stream.Flush();
        }
    }
}