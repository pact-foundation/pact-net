using System.Linq;
using PactNet.Mocks.MockHttpService.Comparers;
using Xunit;

namespace PactNet.Tests.Mocks.MockHttpService.Comparers
{
    public class HttpQueryStringComparerTests
    {
        private HttpQueryStringComparer GetSubject()
        {
            return new HttpQueryStringComparer("query");
        }

        [Fact]
        public void Compare_WithNullExpectedQuery_NoErrorsAreAddedToTheComparisonResult()
        {
            var comparer = GetSubject();

            var result = comparer.Compare(null, "");

            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Compare_WithNullExpectedQueryAndNonNullActualQuery_OneErrorIsAddedToTheComparisonResult()
        {
            const string actualQuery = "test=1234";
            var comparer = GetSubject();

            var result = comparer.Compare(null, actualQuery);

            Assert.Equal(1, result.Errors.Count());
        }

        [Fact]
        public void Compare_WithNonNullExpectedQueryAndNullActualQuery_OneErrorIsAddedToTheComparisonResult()
        {
            const string expectedQuery = "test=1234";
            var comparer = GetSubject();

            var result = comparer.Compare(expectedQuery, null);

            Assert.Equal(1, result.Errors.Count());
        }

        [Fact]
        public void Compare_WithNonEncodedQueryThatMatch_NoErrorsAreAddedToTheComparisonResult()
        {
            const string expected = "test=1234&hello=test";
            const string actual = "test=1234&hello=test";
            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Compare_WithNonEncodedQueryThatDontMatch_OneErrorIsAddedToTheComparisonResult()
        {
            const string expected = "test=1234&hello=test";
            const string actual = "test=1234&hello=Test";
            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(1, result.Errors.Count());
        }

        [Fact]
        public void Compare_WithUrlEncodingCaseInsensitiveMatching_NoErrorsAreAddedToTheComparisonResult()
        {
            const string expected = "2014-08-31T00%3A00%3A00%2B10%3A00";
            const string actual = "2014-08-31T00%3a00%3a00%2b10%3a00";
            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Compare_WithUrlEncodingCaseInsensitiveMatching_OneErrorIsAddedToTheComparisonResult()
        {
            const string expected = "dv=chipbeth%3A00%3A00%2B10%3A00";
            const string actual = "dv=ChipBeth%3a00%3a00%2b10%3a00";
            var comparer = GetSubject();

            var result = comparer.Compare(expected, actual);

            Assert.Equal(1, result.Errors.Count());
        }

    }
}
