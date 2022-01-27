using System;
using System.Threading;

namespace SnapIt.Library.Applications
{
    public class ApplicationInstance
    {
        private static readonly Mutex mutex = new Mutex(true, "{FE4F369C-450C-4FA5-ACCA-3D261A3A7969}");

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
}