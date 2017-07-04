using System.Collections.Generic;
using System.IO;
using PactNet.Extensions;
using PactNet.Infrastructure.Outputters;

namespace PactNet.Core
{
    internal class MockProviderHostConfig : IPactCoreHostConfig
    {
        public string Path { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }
        public IDictionary<string, string> EnvironmentVariables { get; }
        public IEnumerable<IOutput> Outputters { get; }
        private const string RubyVersion = "2.2.0";
        private const string RubyArch = "i386-mingw32";

        public MockProviderHostConfig(int port, bool enableSsl, string providerName, PactConfig config)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var pactCoreDir = $"{currentDir}\\pact";

            var logFile = $"{config.LogDir}{providerName.ToLowerSnakeCase()}_mock_service.log";
            var sslOption = enableSsl ? " --ssl" : "";

            Path = $"{pactCoreDir}\\lib\\ruby\\bin.real\\ruby.exe";
            Arguments = $"-rbundler/setup -I{pactCoreDir}\\lib\\app\\lib \"{pactCoreDir}\\lib\\app\\pact-mock-service.rb\" -p {port} -l \"{FixPathForRuby(logFile)}\" --pact-dir \"{FixPathForRuby(config.PactDir)}\" --pact-specification-version \"{config.SpecificationVersion}\"{sslOption}";
            WaitForExit = false;
            Outputters = config?.Outputters;
            EnvironmentVariables = new Dictionary<string, string>
            {
                { "ROOT_PATH", pactCoreDir },
                { "RUNNING_PATH", $"{pactCoreDir}\\bin\\" },
                { "BUNDLE_GEMFILE", $"{pactCoreDir}\\lib\\vendor\\Gemfile" },
                { "RUBYLIB", $"{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby\\{RubyVersion}\\{RubyArch};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby;{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby\\{RubyVersion}\\{RubyArch};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby;{pactCoreDir}\\lib\\ruby\\lib\\ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\{RubyVersion}\\{RubyArch}" },
                { "SSL_CERT_FILE", $"{pactCoreDir}\\lib\\ruby\\lib\\ca-bundle.crt" }
            };
        }

        //TODO: Make sure everything works with spaces in the paths

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}