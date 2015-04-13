using System.Linq;
using PactNet.Comparers;
using Xunit;

namespace PactNet.Tests.Comparers
{
    public class ComparisonResultTests
    {
        private ComparisonResult GetSubject()
        {
            return new ComparisonResult();
        }

        [Fact]
        public void Failures_WithNestedChildResultsWithFailures_ReturnsAllFailuresFromNestedResultsInCorrectOrder()
        {
            var failure1 = "Failure 1";
            var failure2 = "Failure 2";
            var failure3 = "Failure 3";

            var nestedChildResult2 = new ComparisonResult();
            nestedChildResult2.RecordFailure(new ErrorMessageComparisonFailure(failure3));

            var nestedChildResult = new ComparisonResult();
            nestedChildResult.RecordFailure(new ErrorMessageComparisonFailure(failure2));
            nestedChildResult.AddChildResult(nestedChildResult2);

            var comparisonResult = GetSubject();

            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure(failure1));
            comparisonResult.AddChildResult(nestedChildResult);

            var failures = comparisonResult.Failures;

            Assert.Equal(3, failures.Count());
            Assert.Equal(failure1, failures.First().Result);
            Assert.Equal(failure2, failures.Skip(1).First().Result);
            Assert.Equal(failure3, failures.Last().Result);
        }

        [Fact]
        public void HasFailure_WithOnlyNestedChildResultsWithFailures_ReturnsTrue()
        {
            var nestedChildResult = new ComparisonResult();
            nestedChildResult.RecordFailure(new ErrorMessageComparisonFailure("failure 1"));

            var comparisonResult = GetSubject();

            comparisonResult.AddChildResult(nestedChildResult);

            var hasFailure = comparisonResult.HasFailure;

            Assert.True(hasFailure);
        }

        [Fact]
        public void ShallowFailureCount_WithOnlyNestedChildResultsWithFailures_ReturnsZero()
        {
            var nestedChildResult = new ComparisonResult();
            nestedChildResult.RecordFailure(new ErrorMessageComparisonFailure("failure 1"));

            var comparisonResult = GetSubject();

            comparisonResult.AddChildResult(nestedChildResult);

            var shallowFailureCount = comparisonResult.ShallowFailureCount;

            Assert.Equal(0, shallowFailureCount);
        }

        [Fact]
        public void ShallowFailureCount_WithDirectFailure_ReturnsOne()
        {
            var comparisonResult = GetSubject();

            comparisonResult.RecordFailure(new ErrorMessageComparisonFailure("failure 1"));

            var shallowFailureCount = comparisonResult.ShallowFailureCount;

            Assert.Equal(1, shallowFailureCount);
        }
    }
}
