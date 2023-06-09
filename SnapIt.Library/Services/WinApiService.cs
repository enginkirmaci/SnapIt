﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SnapIt.Library.Entities;
using SnapIt.Library.InteropServices;

namespace SnapIt.Library.Services
{
    public class WinApiService : IWinApiService
    {
        private const int MAX_PATH = 260;

        public IDictionary<IntPtr, string> GetOpenWindows()
        {
            var shellWindow = PInvoke.User32.GetShellWindow();
            var windows = new Dictionary<IntPtr, string>();

            PInvoke.User32.EnumWindows(delegate (IntPtr hwnd, IntPtr lParam)
            {
                if (hwnd == shellWindow) return true;
                if (!PInvoke.User32.IsWindowVisible(hwnd)) return true;

                var length = PInvoke.User32.GetWindowTextLength(hwnd);
                if (length == 0) return true;

                var text = new char[length + 1];
                var finalLength = PInvoke.User32.GetWindowText(hwnd, text, length + 1);
                if (finalLength == 0) return true;

                windows[hwnd] = new string(text, 0, finalLength);
                return true;
            }, IntPtr.Zero);

            return windows;
        }

        public IEnumerable<string> GetOpenWindowsNames()
        {
            return GetOpenWindows().Select(i => i.Value).Distinct().OrderBy(i => i);
        }

        public bool IsAllowedWindowStyle(ActiveWindow activeWindow)
        {
            var style = PInvoke.User32.GetWindowLong(activeWindow.Handle, PInvoke.User32.WindowLongIndexFlags.GWL_STYLE);

            //foreach (WindowStyles ws in (WindowStyles[])Enum.GetValues(typeof(WindowStyles)))
            //{
            //    var wsEnum = (WindowStyles)(style & (uint)(ws));

            //    if (wsEnum != WindowStyles.WS_EX_LEFT)
            //    {
            //    }
            //}
            var windowStyle = (PInvoke.User32.WindowStylesEx)(style & (uint)PInvoke.User32.WindowStylesEx.WS_EX_APPWINDOW);

            return windowStyle == PInvoke.User32.WindowStylesEx.WS_EX_APPWINDOW;
        }

        public bool IsFullscreen(ActiveWindow activeWindow)
        {
            PInvoke.User32.GetWindowRect(PInvoke.User32.GetDesktopWindow(), out PInvoke.RECT desktopWindow);
            var isFullScreen = activeWindow.Boundry.Left == desktopWindow.left &&
                    activeWindow.Boundry.Top == desktopWindow.top &&
                    activeWindow.Boundry.Right == desktopWindow.right &&
                    activeWindow.Boundry.Bottom == desktopWindow.bottom;

            return isFullScreen;
        }

        public void MoveWindow(ActiveWindow activeWindow, Rectangle newRect)
        {
            MoveWindow(activeWindow, newRect.X, newRect.Y, newRect.Width, newRect.Height);
        }

        public bool MoveWindow(ActiveWindow activeWindow, int X, int Y, int width, int height)
        {
            PInvoke.User32.ShowWindow(activeWindow.Handle, PInvoke.User32.WindowShowStyle.SW_SHOWNORMAL); //if window maximized, restores to normal so position can be set

            var res = PInvoke.User32.SetWindowPos(
                activeWindow.Handle,
                PInvoke.User32.SpecialWindowHandles.HWND_TOP,
                X,
                Y,
                width,
                height,
                PInvoke.User32.SetWindowPosFlags.SWP_SHOWWINDOW);

            var msg = Marshal.GetLastWin32Error();
            if (msg != 0)
            {
                //DevMode.Log(msg);
            }
            return res;
        }

        public void SendMessage(ActiveWindow activeWindow)
        {
            PInvoke.User32.SendMessage(activeWindow.Handle, PInvoke.User32.WindowMessage.WM_KEYDOWN, (IntPtr)Keys.Escape, (IntPtr)0);
        }

        public void GetWindowMargin(ActiveWindow activeWindow, out Rectangle withMargin)
        {
            DwmApi.DwmGetWindowAttribute(
                            activeWindow.Handle,
                            DWMWINDOWATTRIBUTE.ExtendedFrameBounds,
                            out withMargin,
                            Marshal.SizeOf(typeof(Rectangle)));
        }

        public ActiveWindow GetActiveWindow()
        {
            var activeWindow = new ActiveWindow
            {
                Handle = PInvoke.User32.GetForegroundWindow()
            };

            var chars = 256;
            var buff = new char[chars + 1];
            var length = PInvoke.User32.GetWindowText(activeWindow.Handle, buff, chars);
            if (length > 0)
            {
                activeWindow.Title = new string(buff, 0, length);
            }

            if (PInvoke.User32.GetWindowRect(activeWindow.Handle, out PInvoke.RECT rct))
            {
                activeWindow.Boundry = new Rectangle(rct.left, rct.top, rct.right, rct.bottom);
            }

            if (activeWindow.Handle == IntPtr.Zero || activeWindow.Boundry.Equals(Rectangle.Empty))
                activeWindow = ActiveWindow.Empty;

            return activeWindow;
        }

        public string GetCurrentDesktopWallpaper()
        {
            var buff = new char[MAX_PATH];

            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);

            PInvoke.User32.SystemParametersInfo(
                PInvoke.User32.SystemParametersInfoAction.SPI_GETDESKWALLPAPER,
                (uint)buff.Length,
                ptr,
                PInvoke.User32.SystemParametersInfoFlags.None);

            var currentWallpaper = new string(buff, 0, buff.Length);
            //User32.SystemParametersInfo(SPI_GETDESKWALLPAPER, currentWallpaper.Length, currentWallpaper, 0);
            return currentWallpaper.Substring(0, currentWallpaper.IndexOf('\0'));
        }
    }
}