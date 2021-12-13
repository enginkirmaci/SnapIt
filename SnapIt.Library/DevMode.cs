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
        public const bool IsActive = false;
        public const bool ShowSnapWindowOnStartup = false;
#else
        public const bool IsActive = false;
        public const bool ShowSnapWindowOnStartup = false;
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