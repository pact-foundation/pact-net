using NSubstitute;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Mocks.MockHttpService.Models;
using PactNet.Reporters;

namespace PactNet.Tests.IntegrationTests.Specification.Models
{
    public class ResponseTestCase : IVerifiable
    {
        private readonly IProviderServiceResponseComparer _responseComparer;
        private readonly IReporter _reporter;

        public bool Match { get; set; }
        public string Comment { get; set; }
        public ProviderServiceResponse Expected { get; set; }
        public ProviderServiceResponse Actual { get; set; }

        public ResponseTestCase()
        {
            _reporter = Substitute.For<IReporter>();
            _responseComparer = new ProviderServiceResponseComparer(_reporter);
        }

        public void Verify()
        {
            _responseComparer.Compare(Expected, Actual);

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