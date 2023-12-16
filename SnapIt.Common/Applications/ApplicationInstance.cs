namespace SnapIt.Common.Applications;

public class ApplicationInstance
{
    private static readonly Mutex mutex = new Mutex(true, "{FF1FFB1E-5D42-4B8F-B42A-52DA1A1964B7}");

    public static bool RegisterSingleInstance()
    {
        if (mutex.WaitOne(TimeSpan.Zero, true))
        {
            mutex.ReleaseMutex();

            return true;
        }

        return false;
    }
}