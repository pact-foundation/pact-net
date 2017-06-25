using PactNet.Core;
using PactNet.Extensions;
using Xunit;

namespace PactNet.Tests.Core
{
    public class MockProviderHostConfigTests
    {
        private IPactCoreHostConfig GetSubject(int port = 2322, bool enableSsl = false, string providerName = "My Test Provider", PactConfig pactConfig = null)
        {
            return new MockProviderHostConfig(port, enableSsl, providerName, pactConfig ?? new PactConfig());
        }

        [Fact]
        public void Ctor_WhenCalled_SetsTheCorrectPath()
        {
            var config = GetSubject();

            Assert.Equal(".\\pact\\bin\\pact-mock-service.bat", config.Path);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsTheCorrectArgs()
        {
            var port = 9332;
            var pactConfig = new PactConfig();
            var providerName = "The best one";

            var config = GetSubject(port, false, providerName, pactConfig);

            var expectedLogFilePath = BuildExpectedLogFilePath(pactConfig.LogDir, providerName);
            var expectedPactDir = BuildExpectedPactDir(pactConfig.PactDir);
            var expectedArguments = BuildExpectedArguments(port, expectedLogFilePath, expectedPactDir, pactConfig.SpecificationVersion);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithSsl_SetsTheCorrectArgs()
        {
            var port = 9332;
            var pactConfig = new PactConfig();
            var providerName = "The best one";
            var enableSsl = true;

            var config = GetSubject(port, enableSsl, providerName, pactConfig);

            var expectedLogFilePath = BuildExpectedLogFilePath(pactConfig.LogDir, providerName);
            var expectedPactDir = BuildExpectedPactDir(pactConfig.PactDir);
            var expectedArguments = BuildExpectedArguments(port, expectedLogFilePath, expectedPactDir, pactConfig.SpecificationVersion, enableSsl);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithNonDefaultLogDirectory_SetsTheCorrectArgs()
        {
            var port = 9332;
            var logDir = "./test";
            var pactConfig = new PactConfig { LogDir = logDir };
            var providerName = "The best one";
            var enableSsl = true;

            var config = GetSubject(port, enableSsl, providerName, pactConfig);

            var expectedLogFilePath = BuildExpectedLogFilePath(logDir + "\\", providerName);
            var expectedPactDir = BuildExpectedPactDir(pactConfig.PactDir);
            var expectedArguments = BuildExpectedArguments(port, expectedLogFilePath, expectedPactDir, pactConfig.SpecificationVersion, enableSsl);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalledWithNonDefaultPactDirectory_SetsTheCorrectArgs()
        {
            var port = 9332;
            var pactDir = "./test";
            var pactConfig = new PactConfig { PactDir = pactDir };
            var providerName = "The best one";
            var enableSsl = true;

            var config = GetSubject(port, enableSsl, providerName, pactConfig);

            var expectedLogFilePath = BuildExpectedLogFilePath(pactConfig.LogDir, providerName);
            var expectedPactDir = BuildExpectedPactDir(pactDir + "\\");
            var expectedArguments = BuildExpectedArguments(port, expectedLogFilePath, expectedPactDir, pactConfig.SpecificationVersion, enableSsl);

            Assert.Equal(expectedArguments, config.Arguments);
        }

        [Fact]
        public void Ctor_WhenCalled_SetsWaitForExitToFalse()
        {
            var config = GetSubject();

            Assert.Equal(false, config.WaitForExit);
        }

        private string BuildExpectedLogFilePath(string logDir, string providerName)
        {
            return ($"{logDir}{providerName.ToLowerSnakeCase()}_mock_service.log").Replace("\\", "/");
        }

        private string BuildExpectedPactDir(string pactDir)
        {
            return pactDir.Replace("\\", "/");
        }

        private string BuildExpectedArguments(
            int port, 
            string logFilePath, 
            string pactFileDir,
            string pactSpecificationVersion,
            bool enableSsl = false)
        {
            var sslOption = enableSsl ? " --ssl" : "";
            return $"-p {port} -l \"{logFilePath}\" --pact-dir \"{pactFileDir}\" --pact-specification-version \"{pactSpecificationVersion}\"{sslOption}";
        }
    }
}
