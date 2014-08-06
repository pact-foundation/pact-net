using NSubstitute;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters;

namespace PactNet.Tests.Specification.Models
{
    public class RequestTestCase : IVerifiable
    {
        private readonly IProviderServiceRequestComparer _requestComparer;
        private readonly IReporter _reporter;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public ProviderServiceRequest Expected { get; set; }
        public ProviderServiceRequest Actual { get; set; }

        public RequestTestCase()
        {
            _reporter = Substitute.For<IReporter>();
            _requestComparer = new ProviderServiceRequestComparer(_reporter);
        }

        public void Verify()
        {
            _requestComparer.Compare(Expected, Actual);

            if (Match)
            {
                _reporter.DidNotReceive().ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
            }
            else
            {
                _reporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
            }
        }
    }
}