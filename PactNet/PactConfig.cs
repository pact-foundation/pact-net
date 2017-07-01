using System.Collections.Generic;
using PactNet.Infrastructure.Output;

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

        public string SpecificationVersion { get; set; }

        public IEnumerable<IOutput> Outputters { get; set; }

        internal string LoggerName;

        public PactConfig()
        {
            PactDir = Constants.DefaultPactDir;
            LogDir = Constants.DefaultLogDir;
            SpecificationVersion = "1.1.0";

            //The output can be directed elsewhere, however there isn't really anything interesting being written here.
            Outputters = new List<IOutput>
            {
                new ConsoleOutput()
            };
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