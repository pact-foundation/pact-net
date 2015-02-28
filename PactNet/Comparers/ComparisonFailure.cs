namespace PactNet.Comparers
{
    public class ComparisonFailure
    {
        public string Message { get; private set; }

        public ComparisonFailure(string message)
        {
            Message = message;
        }
    }
}