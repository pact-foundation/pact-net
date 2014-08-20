namespace PactNet.Tests.Mocks.MockHttpService.Comparers
{
    using System.Linq;

    using NSubstitute;

    using PactNet.Mocks.MockHttpService.Comparers;
    using PactNet.Reporters;

    using Xunit;

    public class HttpQueryStringComparerTests
    {
        private IReporter _mockReporter;

        private HttpQueryStringComparer GetSubject()
        {
            _mockReporter = Substitute.For<IReporter>();
            return new HttpQueryStringComparer("query", _mockReporter);
        }

        [Fact]
        public void Compare_WithUrlEncodingCaseInsensitiveMatching_NoExceptionsAreThrown()
        {
            var expected = "2014-08-31T00%3A00%3A00%2B10%3A00";
            var actual = "2014-08-31T00%3a00%3a00%2b10%3a00";
            var comparer = GetSubject();
            comparer.Compare(expected, actual);
            _mockReporter.DidNotReceive().ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithUrlEncodingCaseInsensitiveMatching_CaseSensitiveNonUrlEncoded()
        {
            var expected = "dv=chipbeth%3A00%3A00%2B10%3A00";
            var actual = "dv=ChipBeth%3a00%3a00%2b10%3a00";
            var comparer = GetSubject();
            comparer.Compare(expected, actual);
            _mockReporter.Received().ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

    }
}
