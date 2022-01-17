#if STANDALONE

using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace SnapScreen.Library.Applications
{
    public class RunAsAdministrator
    {
        public static void Run()
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var test = Application.ExecutablePath;
            var info = new ProcessStartInfo
            {
                Verb = "runas",
                Arguments = "-runas",
                UseShellExecute = true,
                FileName = Application.ExecutablePath // localAppDataPath + @"\microsoft\windowsapps\SnapIt.exe" // path to the appExecutionAlias
            };
            Process.Start(info);
        }

        public static bool IsAdmin(string[] startupArgs)
        {
            return startupArgs.Any(arg => arg.Contains("runas"));
        }
    }
}

#endif