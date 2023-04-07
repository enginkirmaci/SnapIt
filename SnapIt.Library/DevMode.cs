#if DEBUG

using System;
using System.Diagnostics;

#endif

using System.Runtime.CompilerServices;

namespace SnapIt.Library
{
    public static class DevMode
    {
#if DEBUG
        public const bool IsActive = true;
        public const bool ShowSnapWindowOnStartup = false;
        public const bool IsTopmostDisabled = true;
        public const bool SkipLicense = true;
        public const bool TestInTrial = false;
        public const bool SkipRunAsAdmin = true;
#else
        public const bool IsActive = false;
        public const bool ShowSnapWindowOnStartup = false;
        public const bool IsTopmostDisabled = false;
        public const bool SkipLicense = false;
        public const bool TestTrialEnded = false;
        public const bool TestInTrial = false;
        public const bool SkipRunAsAdmin = false;
#endif

        public static void Log(object message = null, bool showTime = false, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
#if DEBUG
            if (!showTime)
            {
                Debug.WriteLine($"Line: {lineNumber}, Method: {caller}, Message: {message}");
            }
            else
            {
                Debug.WriteLine($"{DateTime.Now.ToString("hh.mm.ss.ffffff")} -> Line: {lineNumber}, Method: {caller}, Message: {message}");
            }
#endif
        }
    }
}