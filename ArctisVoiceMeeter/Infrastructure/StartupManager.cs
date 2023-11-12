using System;
using System.Reflection;
using Microsoft.Win32;

namespace NetworkCopy.Utilities
{
    public class StartupManager
    {
        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
        private static readonly string AssemblyLocation = ExecutingAssembly.Location;
        private static readonly string AssemblyRunCommand = AssemblyLocation + " --minimized";

        public bool RunOnStartup
        {
            get => RegistryValues.RunOnStartup == AssemblyRunCommand;
            set => RegistryValues.RunOnStartup = value ? AssemblyRunCommand : null;
        }
    }
}