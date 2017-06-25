using PactNet.Extensions;

namespace PactNet.Core
{
    internal class MockProviderHostConfig : IPactCoreHostConfig
    {
        public string Path { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }

        public MockProviderHostConfig(int port, bool enableSsl, string providerName, PactConfig config)
        {
            var logFile = $"{config.LogDir}{providerName.ToLowerSnakeCase()}_mock_service.log";
            var sslOption = enableSsl ? " --ssl" : "";

            Path = ".\\pact\\bin\\pact-mock-service.bat";
            Arguments = $"-p {port} -l \"{FixPathForRuby(logFile)}\" --pact-dir \"{FixPathForRuby(config.PactDir)}\" --pact-specification-version \"{config.SpecificationVersion}\"{sslOption}";
            WaitForExit = false;
        }

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}