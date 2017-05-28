using System;
using System.Diagnostics;
using System.Management;
using System.Text.RegularExpressions;
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

            var logFile = $"{config.LogDir}\\{providerName.ToLowerSnakeCase()}_mock_service.log";
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

    internal interface IPactProcessConfiguration
    {
        string Path { get; }
        string Arguments { get; }
        bool WaitForExit { get; }
    }

    internal class PactProcessHost<T> where T : IPactProcessConfiguration
    {
        private readonly Process _process;
        private readonly IPactProcessConfiguration _configuration;

        public PactProcessHost(T configuration)
        {
            _configuration = configuration;

            //TODO: Add a way to configure the spec version
            //TODO: Make this work in a cross platform way
            //TODO: Nuget to download this core stuff
            //TODO: Add support for supplying your own ssl cert

            _process = new Process
                       {
                           StartInfo = new ProcessStartInfo
                                       {
                                           WindowStyle = ProcessWindowStyle.Hidden,
                                           FileName = _configuration.Path,
                                           Arguments = _configuration.Arguments,
                                           UseShellExecute = false,
                                           RedirectStandardInput = true,
                                           RedirectStandardOutput = true,
                                           RedirectStandardError = true
                                       }
                       };

            AppDomain.CurrentDomain.DomainUnload += CurrentDomainDomainUnload;
        }

        private void WriteLineToConsole(string data)
        {
            if (data != null)
            {
                Console.WriteLine(Regex.Replace(data, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", ""));
            }
        }

        internal void Start()
        {
            _process.OutputDataReceived += (sender, args) => WriteLineToConsole(args.Data);
            _process.ErrorDataReceived += (sender, args) => WriteLineToConsole(args.Data);

            _process.Start();

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            if (_configuration.WaitForExit)
            {
                _process.WaitForExit();

                if (_process.ExitCode != 0)
                {
                    throw new PactFailureException("Non zero exit code");
                }
            }
        }

        internal void Stop()
        {
            try
            {
                _process.StandardInput.Close();
                KillProcessAndChildren(_process.Id);
                _process.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void CurrentDomainDomainUnload(object sender, EventArgs e)
        {
            Stop();
        }

        private static void KillProcessAndChildren(int pid)
        {
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                var proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }
    }
}