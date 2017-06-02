using System;
using System.Diagnostics;
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

            //TODO: Make this work in a cross platform way
            //TODO: Nuget to download this core stuff
            //TODO: Add support for supplying your own ssl cert

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = _config.Path,
                    Arguments = _config.Arguments,
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

        public void Start()
        {
            _process.OutputDataReceived += (sender, args) => WriteLineToConsole(args.Data);
            _process.ErrorDataReceived += (sender, args) => WriteLineToConsole(args.Data);

            _process.Start();

            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            if (_config.WaitForExit)
            {
                _process.WaitForExit();

                if (_process.ExitCode != 0)
                {
                    throw new PactFailureException("Non zero exit code"); //TODO: Give this a better message
                }
            }
        }

        public void Stop()
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
                // Already exited
            }
        }
    }
}