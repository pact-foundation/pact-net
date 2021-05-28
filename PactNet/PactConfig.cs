using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PactNet.Infrastructure.Outputters;

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

        public IEnumerable<IOutput> Outputters { get; set; } = new List<IOutput>
        {
            new ConsoleOutput()
        };

        /// <summary>
        /// Whether to overwrite pact files completely (default) or merge new interactions into an existing
        /// file if one is found
        /// </summary>
        public bool Overwrite { get; set; } = true;

        /// <summary>
        /// Default JSON serializer settings
        /// </summary>
        public JsonSerializerSettings DefaultJsonSettings { get; set; } = new JsonSerializerSettings();

        public PactConfig()
        {
            PactDir = Constants.DefaultPactDir;
            LogDir = Constants.DefaultLogDir;
        }

        private static string ConvertToDirectory(string path)
        {
            if (!path.EndsWith("/") && !path.EndsWith("\\"))
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }

        /// <summary>
        /// Write a line to all configured outputters
        /// </summary>
        /// <param name="line">Line to write</param>
        public void WriteLine(string line)
        {
            foreach (IOutput output in this.Outputters)
            {
                output.WriteLine(line);
            }
        }
    }
}