namespace PactNet.Matchers
{
    internal class FailedMatcherCheck : MatcherCheck
    {
        public MatcherCheckFailureType FailureType { get; private set; }

        public FailedMatcherCheck()
        {
        }

        public FailedMatcherCheck(string path, MatcherCheckFailureType failureType)
            :base(path)
        {
            this.FailureType = failureType;
        }

        public FailedMatcherCheck(string path, MatcherCheckFailureType failureType, object expected, object actual)
            :base(path, expected, actual)
        {
            this.FailureType = failureType;
        }

        public override string ToString()
        {
            return string.Format("{0}\t{1}",this.FailureType, base.ToString());
        }
    }
}