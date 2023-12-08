namespace SnapIt.Common.Applications;

public class RunAsAdministrator
{
    public static void Run()
    {
        var info = new ProcessStartInfo
        {
            Verb = "runas",
            Arguments = "-runas",
            UseShellExecute = true,
            FileName = Environment.ProcessPath // Application.ExecutablePath; // localAppDataPath + @"\microsoft\windowsapps\SnapIt.exe" // path to the appExecutionAlias
        };
        Process.Start(info);
    }

    public static bool IsAdmin(string[] startupArgs)
    {
        return startupArgs.Any(arg => arg.Contains("runas"));
    }
}