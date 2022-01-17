using System;
using System.Drawing;
using System.Runtime.InteropServices;
using SnapScreen.Library.Entities;
using WpfScreenHelper;

namespace SnapIt.Library.Extensions
{
    //TODO refactor here
    public class DpiHelper
    {
        public static Dpi GetDpiFromPoint(int X, int Y)
        {
            var pnt = new Point(X + 1, Y + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);

            uint dpiX = 1;
            uint dpiY = 1;

            try
            {
                GetDpiForMonitor(mon, DpiType.Effective, out dpiX, out dpiY);
            }
            catch (Exception)
            {
                dpiX = 1;
                dpiY = 1;
            }

            return new Dpi { X = 96.0 / dpiX, Y = 96.0 / dpiY };
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
    }

    public static class ScreenExtensions
    {
        public static void GetDpi(this Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
        {
            var pnt = new Point((int)screen.Bounds.Left + 1, (int)screen.Bounds.Top + 1);
            var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);

            try
            {
                GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
            }
            catch (Exception)
            {
                dpiX = 1;
                dpiY = 1;
            }
        }

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
        [DllImport("User32.dll")]
        private static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        [DllImport("Shcore.dll")]
        private static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }
}