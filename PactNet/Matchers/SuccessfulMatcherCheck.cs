namespace PactNet.Matchers
{
    internal class SuccessfulMatcherCheck : MatcherCheck
    {
        public SuccessfulMatcherCheck()
        {
        }

        public SuccessfulMatcherCheck(string path)
            :base(path)
        {
        }

        public SuccessfulMatcherCheck(string path, object expected, object actual)
            :base(path, expected, actual)
        {
        }
    }
}