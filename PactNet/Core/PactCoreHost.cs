using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PactNet.Models;

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
            var pactCoreDir = $"{currentDir}\\"; //OS specific version will be appended

            var platform = Platform.Windows;

#if !USE_NET4X
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                platform = Platform.Windows;
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX))
            {
                platform = Platform.Osx;
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) &&
                System.Runtime.InteropServices.RuntimeInformation.OSArchitecture == System.Runtime.InteropServices.Architecture.X86)
            {
                platform = Platform.LinuxX86;
            }
            else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) &&
                     System.Runtime.InteropServices.RuntimeInformation.OSArchitecture == System.Runtime.InteropServices.Architecture.X64)
            {
                platform = Platform.LinuxX64;
            }
            else
            {
                throw new PactFailureException("Sorry your current OS platform or architecture is not supported");
            }
#endif

            switch (platform)
            {
                case Platform.Windows:
                    pactCoreDir += "pact-win32";
                    break;
                case Platform.Osx:
                    pactCoreDir += "pact-osx";
                    break;
                case Platform.LinuxX86:
                    pactCoreDir += "pact-linux-x86";
                    break;
                case Platform.LinuxX64:
                    pactCoreDir += "pact-linux-x86_64";
                    break;
            }

            if (!Directory.Exists(pactCoreDir))
            {
                //TODO: Fall back to using the locally install ruby and packaged assets
            }

            var startInfo = new ProcessStartInfo
            {
#if USE_NET4X
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

            var envVars = new Dictionary<string, string>
            {
                { "ROOT_PATH", pactCoreDir },
                { "RUNNING_PATH", $"{pactCoreDir}\\bin\\" },
                { "BUNDLE_GEMFILE", $"{pactCoreDir}\\lib\\vendor\\Gemfile" },
                { "RUBYLIB", $"{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby\\{RubyVersion}\\{RubyArch};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\site_ruby;{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby\\{RubyVersion}\\{RubyArch};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\vendor_ruby;{pactCoreDir}\\lib\\ruby\\lib\\ruby\\{RubyVersion};{pactCoreDir}\\lib\\ruby\\lib\\ruby\\{RubyVersion}\\{RubyArch}" },
                { "SSL_CERT_FILE", $"{pactCoreDir}\\lib\\ruby\\lib\\ca-bundle.crt" }
            };

            foreach (var envVar in envVars)
            {
#if USE_NET4X
                startInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
#else
                startInfo.Environment[envVar.Key] = envVar.Value;
#endif
            }

            _process = new Process
            {
                StartInfo = startInfo
            };

#if USE_NET4X
            AppDomain.CurrentDomain.DomainUnload += (o, e) => Stop();
#else
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += context => Stop();
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