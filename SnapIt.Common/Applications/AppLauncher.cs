namespace SnapIt.Common.Applications;

public class AppLauncher
{
    public static void RunAsAdmin()
    {
        Run(true);
    }

    public static void Run()
    {
        Run(false);
    }

    public static bool IsAdmin(string[] startupArgs)
    {
        return startupArgs.Any(arg => arg.Contains("runas"));
    }

    private static void Run(bool asAdmin)
    {
        var info = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = Environment.ProcessPath // Application.ExecutablePath; // localAppDataPath + @"\microsoft\windowsapps\SnapIt.exe" // path to the appExecutionAlias
        };

        if (asAdmin)
        {
            info.Verb = "runas";
            info.Arguments = "-runas";
        }

        Process.Start(info);
    }
}