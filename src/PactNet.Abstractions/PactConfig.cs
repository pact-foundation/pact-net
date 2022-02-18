using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PactNet.Infrastructure.Outputters;

namespace PactNet
{
    /// <summary>
    /// Pact configuration
    /// </summary>
    public class PactConfig
    {
        private string pactDir;

        /// <summary>
        /// Pact file destination directory
        /// </summary>
        public string PactDir
        {
            get { return this.pactDir; }
            set { this.pactDir = ConvertToDirectory(value); }
        }

        /// <summary>
        /// Log level for the verifier
        /// </summary>
        public PactLogLevel LogLevel { get; set; } = PactLogLevel.Information;

        /// <summary>
        /// Log outputs
        /// </summary>
        public IEnumerable<IOutput> Outputters { get; set; } = new List<IOutput>
        {
            new ConsoleOutput()
        };

        /// <summary>
        /// Default JSON serializer settings
        /// </summary>
        public JsonSerializerSettings DefaultJsonSettings { get; set; } = new JsonSerializerSettings();

        /// <summary>
        /// Initialises a new instance of the <see cref="PactConfig"/> class.
        /// </summary>
        public PactConfig()
        {
            this.PactDir = Constants.DefaultPactDir;
        }

        /// <summary>
        /// Ensure a string path ends with a trailing slash
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <returns>Path with trailing slash</returns>
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
