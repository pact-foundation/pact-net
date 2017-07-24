using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PactNet.Core
{
    internal class PactCoreHost<T> : IPactCoreHost where T : IPactCoreHostConfig
    {
        private readonly Process _process;
        private readonly IPactCoreHostConfig _config;
        private const string RubyVersion = "2.2.0";
        private const string RubyArch = "i386-mingw32";

        public PactCoreHost(T config)
        {
            _config = config;

            var currentDir = Directory.GetCurrentDirectory();
            var pactCoreDir = $"{currentDir}\\pact";

            var startInfo = new ProcessStartInfo
            {
#if !NETSTANDARD1_5
                WindowStyle = ProcessWindowStyle.Hidden,
#endif
                FileName = $"{pactCoreDir}\\lib\\ruby\\bin.real\\ruby.exe",
                Arguments = $"-rbundler/setup -I\"{pactCoreDir}\\lib\\app\\lib\" \"{pactCoreDir}\\lib\\app\\{_config.Script}\" {_config.Arguments}",
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            startInfo.Environment["ROOT_PATH"] = pactCoreDir;
            startInfo.Environment["RUNNING_PATH"] = $"{pactCoreDir}\\bin\\";
            startInfo.Environment["BUNDLE_GEMFILE"] = $"{pactCoreDir}\\lib\\vendor\\Gemfile";
            startInfo.Environment["RUBYLIB"] = $"{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby\\{RubyVersion}\\{RubyArch};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby;{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby\\{RubyVersion}\\{RubyArch};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby;{pactCoreDir}\\lib\\ruby\\lib\\ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\{RubyVersion}\\{RubyArch}";
            startInfo.Environment["SSL_CERT_FILE"] = $"{pactCoreDir}\\lib\\ruby\\lib\\ca-bundle.crt";
            
            _process = new Process
            {
                StartInfo = startInfo
            };

#if NETSTANDARD1_5
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += context => Stop();
#else
            AppDomain.CurrentDomain.DomainUnload += (o, e) => Stop();
#endif
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
            bool hasExited;

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
                    _process.WaitForExit();
                    _process.Dispose();
                }
                catch (Exception)
                {
                    throw new PactFailureException("Could not terminate the Pact Core Host, please manually kill the 'Ruby interpreter' process");
                }
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