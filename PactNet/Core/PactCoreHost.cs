using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
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
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                    //NOTE: Do Not set CreateNoWindow = true, as it will spawn the ruby process as a child and we then can't kill it without using System.Management
                }
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
            try
            {
                _process.OutputDataReceived -= WriteLineToOutput;
                _process.ErrorDataReceived -= WriteLineToOutput;
                _process.CancelOutputRead();
                _process.CancelErrorRead();
                KillProcessAndChildren(_process.Id);
            }
            catch (Exception)
            {
                WriteToOutputters("Could not terminate the Pact Core Host please manually kill the 'Ruby interpreter' process");
            }
        }

        private void CurrentDomainUnload(object sender, EventArgs e)
        {
            Stop();
        }

        private void KillProcessAndChildren(int pid)
        {
            WriteToOutputters($"Killing PID: {pid}");

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
                // Already exited
            }
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