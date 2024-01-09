using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;

namespace NetworkCopy.Utilities
{
    public class StartupManager
    {
        private static readonly string AssemblyLocation = Process.GetCurrentProcess().MainModule.FileName;
        private static readonly string AssemblyRunCommand = $"\"{AssemblyLocation}\" --minimized";

        public bool RunOnStartup
        {
            get => RegistryValues.RunOnStartup == AssemblyRunCommand;
            set => RegistryValues.RunOnStartup = value ? AssemblyRunCommand : null;
        }
    }
}