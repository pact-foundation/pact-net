namespace PactNet.Comparers
{
    internal class ErrorMessageComparisonFailure : ComparisonFailure
    {
        public ErrorMessageComparisonFailure(string errorMessage)
        {
            Result = errorMessage;
        }
    }
}