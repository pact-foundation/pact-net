namespace PactNet
{
    public class PactVerifierConfig
    {
        public string LogDir { get; set; }

        internal string LoggerName;

        public PactVerifierConfig()
        {
            LogDir = Constants.DefaultLogDir;
        }
    }
}