using System.Reflection;
using System.Security.Permissions;
using Microsoft.Win32;

namespace NetworkCopy.Utilities
{
	public class RegistryValues
	{
		private static readonly string StartupKeyPath =
			"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

		private static readonly string StartupValueName =
			Assembly.GetExecutingAssembly().GetName().Name;

		public static string RunOnStartup
		{
			get => GetValue(StartupKeyPath, StartupValueName);
			set => SetValue(StartupKeyPath, StartupValueName, value);
		}

		private static RegistryKey GetKey(string keyPath,
			bool writable = false)
		{
			return Registry.CurrentUser.CreateSubKey(keyPath,
				writable ? RegistryKeyPermissionCheck.ReadWriteSubTree : RegistryKeyPermissionCheck.ReadSubTree);
		}

		private static string GetValue(string keyPath, string valueName)
		{
			using (var key = GetKey(keyPath))
			{
				return key.GetValue(valueName) as string;
			}
		}

		private static void SetValue(string keyPath, string valueName, string value)
		{
			using (var key = GetKey(keyPath, true))
			{
				if (value == null)
				{
					key.DeleteValue(valueName, false);
				}
				else
				{
					key.SetValue(valueName, value);
				}
			}
		}
	}
}