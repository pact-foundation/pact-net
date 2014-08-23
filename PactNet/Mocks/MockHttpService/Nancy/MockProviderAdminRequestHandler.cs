using System;
using System.Linq;
using Nancy;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Reporters;

namespace PactNet.Mocks.MockHttpService.Nancy
{
    public class MockProviderAdminRequestHandler : IMockProviderAdminRequestHandler
    {
        private readonly IStatsProvider _statsProvider;
        private readonly IProviderServiceRequestComparer _requestComparer;
        private readonly IReporter _reporter = new Reporter();

        [Obsolete("For testing only.")]
        public MockProviderAdminRequestHandler(
            IStatsProvider statsProvider,
            IReporter reporter,
            IProviderServiceRequestComparer requestComparer)
        {
            _statsProvider = statsProvider;
            _reporter = reporter;
            _requestComparer = requestComparer;
        }

        public MockProviderAdminRequestHandler(
            IStatsProvider statsProvider,
            IReporter reporter)
            : this(statsProvider,
                   reporter,
                   new ProviderServiceRequestComparer(reporter))
        {
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
                _statsProvider.Clear();

                return new Response
                {
                    StatusCode = HttpStatusCode.OK
                };
            }

            if (context.Request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase) &&
                context.Request.Path == "/interactions/verification")
            {
                if (_statsProvider == null)
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }

                if (_statsProvider.Stats != null && _statsProvider.Stats.Any())
                {
                    //Check number of calls

                    //Check actual request against matching request
                    foreach (var stat in _statsProvider.Stats)
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

            return new Response
            {
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }
}