using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace PactNet.Core
{
    internal class PactCoreHost<T> : IPactCoreHost where T : IPactCoreHostConfig
    {
        private readonly Process _process;
        private readonly IPactCoreHostConfig _config;

        public PactCoreHost(T config)
        {
            _config = config;

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = _config.Path,
                Arguments = _config.Arguments,
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            if (_config.EnvironmentVariables != null)
            {
                foreach (var ev in _config.EnvironmentVariables)
                {
                    startInfo.EnvironmentVariables.Add(ev.Key, ev.Value);
                }
            }

            _process = new Process
            {
                StartInfo = startInfo
            };

            AppDomain.CurrentDomain.DomainUnload += CurrentDomainUnload;
        }

        public void Start()
        {
            _process.OutputDataReceived += WriteLineToOutput;
            _process.ErrorDataReceived += WriteLineToOutput;

            var success = _process.Start();

            if (!success)
            {
                throw new PactFailureException("Could not start the Pact Core Host");
            }

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            if (_config.WaitForExit)
            {
                _process.WaitForExit();

                if (_process.ExitCode != 0)
                {
                    throw new PactFailureException("Pact verification failed. See output for details. \nIf the output is empty please provide a custom config.Outputters (IOutput) for your test framework, as we couldn't write to the console.");
                }
            }
        }

        public void Stop()
        {
            var hasExited = false;

            try
            {
                hasExited = _process.HasExited;
            }
            catch (InvalidOperationException)
            {
                hasExited = true;
            }

            if (!hasExited)
            {
                try
                {
                    _process.OutputDataReceived -= WriteLineToOutput;
                    _process.ErrorDataReceived -= WriteLineToOutput;
                    _process.CancelOutputRead();
                    _process.CancelErrorRead();
                    _process.Kill();
                    _process.Dispose();
                }
                catch (Exception)
                {
                    throw new PactFailureException("Could not terminate the Pact Core Host, please manually kill the 'Ruby interpreter' process");
                }
            }
        }

        private void CurrentDomainUnload(object sender, EventArgs e)
        {
            Stop();
        }

        private void WriteLineToOutput(object sender, DataReceivedEventArgs eventArgs)
        {
            if (eventArgs.Data != null)
            {
                WriteToOutputters(Regex.Replace(eventArgs.Data, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", ""));
            }
        }

        private void WriteToOutputters(string line)
        {
            if (_config.Outputters != null && _config.Outputters.Any())
            {
                foreach (var output in _config.Outputters)
                {
                    output.WriteLine(line);
                }
            }
        }
    }
}