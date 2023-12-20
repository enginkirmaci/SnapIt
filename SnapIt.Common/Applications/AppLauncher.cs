namespace SnapIt.Common.Applications;

public class AppLauncher
{
    public static void RunAsAdmin()
    {
        Run("runas", true);
    }

    public static void RunBypassSingleInstance()
    {
        Run("nosingle");
    }

    public static bool IsAdmin(string[] startupArgs)
    {
        return startupArgs.Any(arg => arg.Contains("runas"));
    }

    public static bool BypassSingleInstance(string[] startupArgs)
    {
        return startupArgs.Any(arg => arg.Contains("nosingle"));
    }

    private static void Run(string argument = null, bool useShellExecute = false)
    {
        var info = new ProcessStartInfo
        {
            UseShellExecute = useShellExecute,
            FileName = Process.GetCurrentProcess().ProcessName // Application.ExecutablePath; // localAppDataPath + @"\microsoft\windowsapps\SnapIt.exe" // path to the appExecutionAlias
        };

        if (!string.IsNullOrEmpty(argument))
        {
            info.Verb = argument;
            info.Arguments = $"-{argument}";
        }

        Process.Start(info);
    }
}