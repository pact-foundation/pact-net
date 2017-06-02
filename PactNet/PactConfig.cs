namespace PactNet
{
    public class PactConfig
    {
        private string _pactDir;
        public string PactDir
        {
            get { return _pactDir; }
            set { _pactDir = ConvertToDirectory(value); }
        }

        private string _logDir;
        public string LogDir
        {
            get { return _logDir; }
            set { _logDir = ConvertToDirectory(value); }
        }

        public string SpecificationVersion { get; set; } //TODO: Maybe this is better as an enum?

        internal string LoggerName;

        public PactConfig()
        {
            PactDir = Constants.DefaultPactDir;
            LogDir = Constants.DefaultLogDir;
            SpecificationVersion = "1.1.0";
        }

        private static string ConvertToDirectory(string path)
        {
            if (!path.EndsWith("/") && !path.EndsWith("\\"))
            {
                return path + "\\";
            }

            return path;
        }
    }
}