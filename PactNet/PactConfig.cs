namespace PactNet
{
    public class PactConfig
    {
        public string PactDir { get; set; }
        public string LogDir { get; set; }

        internal string LoggerName;

        public PactConfig()
        {
            PactDir = Constants.DefaultPactDir;
            LogDir = Constants.DefaultLogDir;
        }
    }
}