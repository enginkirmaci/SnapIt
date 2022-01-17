using System;
using PInvoke;
using SnapScreen.Library.Entities;
using WpfScreenHelper;

namespace SnapIt.Library.Extensions
{
    //TODO refactor here
    public class DpiHelper
    {
        public static Dpi GetDpiFromPoint(int X, int Y)
        {
            var pnt = new POINT { x = X + 1, y = Y + 1 };
            var mon = User32.MonitorFromPoint(pnt, User32.MonitorOptions.MONITOR_DEFAULTTONEAREST)
;
            int dpiY;
            int dpiX;

            try
            {
                SHCore.GetDpiForMonitor(mon, MONITOR_DPI_TYPE.MDT_EFFECTIVE_DPI, out dpiX, out dpiY);
            }
            catch (Exception)
            {
                dpiX = 1;
                dpiY = 1;
            }

            return new Dpi { X = 96.0 / dpiX, Y = 96.0 / dpiY };
        }
    }

    public static class ScreenExtensions
    {
        public static Dpi GetDpi(this Screen screen)
        {
            return DpiHelper.GetDpiFromPoint((int)screen.Bounds.Left, (int)screen.Bounds.Top);
        }
    }

    //https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    public enum DpiType
    {
        Effective = 0,
        Angular = 1,
        Raw = 2,
    }
}