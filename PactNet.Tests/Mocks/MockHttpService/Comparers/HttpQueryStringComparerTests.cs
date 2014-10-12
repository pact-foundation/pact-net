using NSubstitute;
using PactNet.Mocks.MockHttpService.Comparers;
using PactNet.Reporters;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Comparers
{
    public class HttpQueryStringComparerTests
    {
        private IReporter _mockReporter;

        private HttpQueryStringComparer GetSubject()
        {
            _mockReporter = Substitute.For<IReporter>();
            return new HttpQueryStringComparer("query", _mockReporter);
        }

        [Fact]
        public void Compare_WithNullExpectedQuery_ReportErrorIsNotCalledOnTheReporter()
        {
            var comparer = GetSubject();

            comparer.Compare(null, "");

            _mockReporter.DidNotReceive().ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithNullExpectedQueryAndNonNullActualQuery_ReportErrorIsCalledOnTheReporter()
        {
            const string actualQuery = "test=1234";
            var comparer = GetSubject();

            comparer.Compare(null, actualQuery);

            _mockReporter.Received(1).ReportError(null, null, actualQuery);
        }

        [Fact]
        public void Compare_WithNonNullExpectedQueryAndNullActualQuery_ReportErrorIsCalledOnTheReporter()
        {
            const string expectedQuery = "test=1234";
            var comparer = GetSubject();

            comparer.Compare(expectedQuery, null);

            _mockReporter.Received(1).ReportError(null, expectedQuery, null);
        }

        [Fact]
        public void Compare_WithNonEncodedQueryThatMatch_ReportErrorIsNotCalledOnTheReporter()
        {
            const string expected = "test=1234&hello=test";
            const string actual = "test=1234&hello=test";
            var comparer = GetSubject();

            comparer.Compare(expected, actual);

            _mockReporter.DidNotReceive().ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithNonEncodedQueryThatDontMatch_ReportErrorIsCalledOnTheReporter()
        {
            const string expected = "test=1234&hello=test";
            const string actual = "test=1234&hello=Test";
            var comparer = GetSubject();

            comparer.Compare(expected, actual);

            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithUrlEncodingCaseInsensitiveMatching_ReportErrorIsNotCalledOnTheReporter()
        {
            const string expected = "2014-08-31T00%3A00%3A00%2B10%3A00";
            const string actual = "2014-08-31T00%3a00%3a00%2b10%3a00";
            var comparer = GetSubject();

            comparer.Compare(expected, actual);

            _mockReporter.DidNotReceive().ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

        [Fact]
        public void Compare_WithUrlEncodingCaseInsensitiveMatching_ReportErrorIsCalledOnTheReporter()
        {
            const string expected = "dv=chipbeth%3A00%3A00%2B10%3A00";
            const string actual = "dv=ChipBeth%3a00%3a00%2b10%3a00";
            var comparer = GetSubject();

            comparer.Compare(expected, actual);

            _mockReporter.Received(1).ReportError(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<object>());
        }

    }
}
