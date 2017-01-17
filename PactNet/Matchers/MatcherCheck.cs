namespace PactNet.Matchers
{
    public abstract class MatcherCheck
    {
        private const string PathPrefix = "$.";
        private string _path;

        public string Path
        {
            get { return _path; }
            protected set { _path = value.StartsWith(PathPrefix) ? value : PathPrefix + value; }
        }
    }
}