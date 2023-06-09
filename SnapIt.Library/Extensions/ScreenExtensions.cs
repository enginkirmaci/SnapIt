﻿using System;
using PInvoke;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Extensions
{
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
        public static Dpi GetDpi(this SnapScreen screen)
        {
            return DpiHelper.GetDpiFromPoint((int)screen.Bounds.Left, (int)screen.Bounds.Top);
        }
    }
}