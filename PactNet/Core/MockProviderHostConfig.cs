using System.Collections.Generic;
using PactNet.Extensions;
using PactNet.Infrastructure.Outputters;

namespace PactNet.Core
{
    internal class MockProviderHostConfig : IPactCoreHostConfig
    {
        public string Script { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }
        public IEnumerable<IOutput> Outputters { get; }

        public MockProviderHostConfig(int port, bool enableSsl, string providerName, PactConfig config)
        {
            var logFile = $"{config.LogDir}{providerName.ToLowerSnakeCase()}_mock_service.log";
            var sslOption = enableSsl ? " --ssl" : "";

            Script = "pact-mock-service";
            Arguments = $"-p {port} -l \"{FixPathForRuby(logFile)}\" --pact-dir \"{FixPathForRuby(config.PactDir)}\" --pact-specification-version \"{config.SpecificationVersion}\"{sslOption}";
            WaitForExit = false;
            Outputters = config?.Outputters;
        }

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}