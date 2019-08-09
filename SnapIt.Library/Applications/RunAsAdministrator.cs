using System;
using System.Diagnostics;
using System.Linq;

namespace SnapIt.Library.Applications
{
	public class RunAsAdministrator
	{
		public static void Run()
		{
			var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

			ProcessStartInfo info = new ProcessStartInfo
			{
				Verb = "runas",
				Arguments = "-runas",
				UseShellExecute = true,
				FileName = localAppDataPath + @"\microsoft\windowsapps\SnapIt.exe" // path to the appExecutionAlias
			};
			Process.Start(info);
		}

		public static bool IsAdmin(string[] startupArgs)
		{
			return startupArgs.Any(arg => arg.Contains("runas"));
		}
	}
}