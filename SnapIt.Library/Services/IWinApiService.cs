﻿using SnapIt.Library.Entities;
using System;
using System.Collections.Generic;

namespace SnapIt.Library.Services
{
    public interface IWinApiService
    {
        IDictionary<IntPtr, string> GetOpenWindows();

        IEnumerable<string> GetOpenWindowsNames();

        bool IsFullscreen(ActiveWindow activeWindow);

        bool IsAllowedWindowStyle(ActiveWindow activeWindow);

        void MoveWindow(ActiveWindow activeWindow, Rectangle newRect);

        bool MoveWindow(ActiveWindow activeWindow, int X, int Y, int width, int height);

        void SendMessage(ActiveWindow activeWindow);

        void GetWindowMargin(ActiveWindow activeWindow, out Rectangle withMargin);

        ActiveWindow GetActiveWindow();

        string GetCurrentDesktopWallpaper();
    }
}