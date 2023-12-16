using PInvoke;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

namespace SnapIt.Common.Extensions;

public class DpiHelper
{
    public static Dpi GetDpiFromPoint(float X, float Y)
    {
        var pnt = new POINT { x = (int)X + 1, y = (int)Y + 1 };
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

        return new Dpi { X = 96.0f / dpiX, Y = 96.0f / dpiY };
    }
}

public static class ScreenExtensions
{
    public static Dpi GetDpi(this SnapScreen screen)
    {
        return DpiHelper.GetDpiFromPoint((int)screen.Bounds.Left, (int)screen.Bounds.Top);
    }
}