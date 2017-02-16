namespace PactNet.Matchers
{
    public abstract class MatcherCheck
    {
        private const string PathPrefix = "$.";
        private string _path;
        private object _expected;
        private object _actual;

        protected MatcherCheck()
        {
        }

        protected MatcherCheck(string path)
        {
            this.Path = path;
        }

        protected MatcherCheck(string path, object expected, object actual)
        {
            this.Path = path;
            this.Expected = expected;
            this.Actual = actual;
        }

        public string Path
        {
            get { return _path; }
            protected set { _path = value.StartsWith(PathPrefix) ? value : PathPrefix + value; }
        }

        public object Expected
        {
            get { return _expected; }
            protected set { _expected = value; }
        }

        public object Actual
        {
            get { return _actual; }
            protected set { _actual = value; }
        }

        public override string ToString()
        {
            return string.Format("{0}\tExpected: {1}, Actual: {2}", _path, _expected ?? "(null)", _actual ?? "(null)");
        }
    }
}