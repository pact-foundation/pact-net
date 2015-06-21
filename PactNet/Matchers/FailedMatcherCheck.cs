namespace PactNet.Matchers
{
    internal class FailedMatcherCheck : MatcherCheck
    {
        public MatcherCheckFailureType FailureType { get; private set; }

        public FailedMatcherCheck(string path, MatcherCheckFailureType failureType)
        {
            Path = path;
            FailureType = failureType;
        }
    }
}