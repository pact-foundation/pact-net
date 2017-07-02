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

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Minimized,
                    FileName = _config.Path,
                    Arguments = _config.Arguments,
                    UseShellExecute = false, //Important so that the correct process is killed
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                    //NOTE: Do Not set CreateNoWindow = true, as it will spawn the ruby process as a child and we then can't kill it without using System.Management
                }
            };

            AppDomain.CurrentDomain.DomainUnload += CurrentDomainUnload;
        }

        private void WriteLineToOutput(object sender, DataReceivedEventArgs eventArgs)
        {
            if (eventArgs.Data != null)
            {
                if (_config.Outputters != null && _config.Outputters.Any())
                {
                    foreach (var output in _config.Outputters)
                    {
                        output.WriteLine(Regex.Replace(eventArgs.Data, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", ""));
                    }
                }
            }
        }

        public void Start()
        {
            _process.OutputDataReceived += WriteLineToOutput;
            _process.ErrorDataReceived += WriteLineToOutput;

            _process.Start();

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
            if (!_process.HasExited)
            {
                try
                {
                    _process.OutputDataReceived -= WriteLineToOutput;
                    _process.ErrorDataReceived -= WriteLineToOutput;
                    _process.CancelOutputRead();
                    _process.CancelErrorRead();
                    _process.CloseMainWindow();
                    _process.Close();
                    _process.Dispose();
                }
                catch (Exception)
                {
                    try
                    {
                        _process.Kill();
                    }
                    catch (Exception)
                    {
                        throw new PactFailureException("Could not terminate the pact core host, please manually kill the 'Ruby interpreter' process");
                    }
                }
            }
        }

        private void CurrentDomainUnload(object sender, EventArgs e)
        {
            Stop();
        }
    }
}