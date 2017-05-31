using PactNet.Extensions;

namespace PactNet.Core
{
    internal class MockProviderConfiguration : IPactProcessConfiguration
    {
        public string Path { get; }
        public string Arguments { get; }
        public bool WaitForExit { get; }

        public MockProviderConfiguration(int port, bool enableSsl, string providerName, PactConfig config)
        {
            config.SpecificationVersion = "2.0.0"; //TODO: Remove this

            var logFile = $"{config.LogDir}{providerName.ToLowerSnakeCase()}_mock_service.log";
            var sslOption = enableSsl ? " --ssl" : "";

            Path = "C:\\src\\os\\concord\\PactNet\\Core\\pact-mock-service-win32\\bin\\pact-mock-service.bat";
            Arguments = $"-p {port} -l \"{FixPathForRuby(logFile)}\" --pact-dir \"{FixPathForRuby(config.PactDir)}\" --pact-specification-version \"{config.SpecificationVersion}\"{sslOption}";
            WaitForExit = false;
        }

        private string FixPathForRuby(string path)
        {
            return path.Replace("\\", "/");
        }
    }
}