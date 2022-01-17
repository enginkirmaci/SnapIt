using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SnapScreen.Library.Entities;
using SnapScreen.Library.InteropServices;

namespace SnapScreen.Library.Services
{
    public class WinApiService : IWinApiService
    {
        private const uint SPI_GETDESKWALLPAPER = 0x73;
        private const int MAX_PATH = 260;

        public IDictionary<IntPtr, string> GetOpenWindows()
        {
            var shellWindow = User32.GetShellWindow();
            var windows = new Dictionary<IntPtr, string>();

            User32.EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!User32.IsWindowVisible(hWnd)) return true;

                var length = User32.GetWindowTextLength(hWnd);
                if (length == 0) return true;

                var builder = new StringBuilder(length);
                User32.GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;
            }, 0);

            return windows;
        }

        public IEnumerable<string> GetOpenWindowsNames()
        {
            return GetOpenWindows().Select(i => i.Value).Distinct().OrderBy(i => i);
        }

        public bool IsAllowedWindowStyle(ActiveWindow activeWindow)
        {
            var style = User32.GetWindowLong(activeWindow.Handle, (int)GWL.STYLE);

            //foreach (WindowStyles ws in (WindowStyles[])Enum.GetValues(typeof(WindowStyles)))
            //{
            //    var wsEnum = (WindowStyles)(style & (uint)(ws));

            //    if (wsEnum != WindowStyles.WS_EX_LEFT)
            //    {
            //    }
            //}

            var windowStyle = (WindowStyles)(style & (uint)WindowStyles.WS_EX_APPWINDOW);

            return windowStyle == WindowStyles.WS_EX_APPWINDOW;
        }

        public bool IsFullscreen(ActiveWindow activeWindow)
        {
            User32.GetWindowRect(User32.GetDesktopWindow(), out Rectangle desktopWindow);
            var isFullScreen = activeWindow.Boundry.Left == desktopWindow.Left &&
                    activeWindow.Boundry.Top == desktopWindow.Top &&
                    activeWindow.Boundry.Right == desktopWindow.Right &&
                    activeWindow.Boundry.Bottom == desktopWindow.Bottom;

            return isFullScreen;
        }

        public void MoveWindow(ActiveWindow activeWindow, Rectangle newRect)
        {
            MoveWindow(activeWindow, newRect.X, newRect.Y, newRect.Width, newRect.Height);
        }

        public bool MoveWindow(ActiveWindow activeWindow, int X, int Y, int width, int height)
        {
            User32.ShowWindow(activeWindow.Handle, (int)ShowWindowCommand.SW_SHOWNORMAL); //if window maximized, restores to normal so position can be set

            var res = User32.SetWindowPos(
                activeWindow.Handle,
                (IntPtr)SpecialWindowHandles.HWND_TOP,
                X,
                Y,
                width,
                height,
                SetWindowPosFlags.ShowWindow);

            var msg = Marshal.GetLastWin32Error();
            if (msg != 0)
            {
                //DevMode.Log(msg);
            }
            return res;
        }

        public void SendMessage(ActiveWindow activeWindow)
        {
            const uint WM_KEYDOWN = 0x100;

            User32.SendMessage(activeWindow.Handle, WM_KEYDOWN, (IntPtr)Keys.Escape, (IntPtr)0);
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
                Handle = User32.GetForegroundWindow()
            };

            var chars = 256;
            var buff = new StringBuilder(chars);
            if (User32.GetWindowText(activeWindow.Handle, buff, chars) > 0)
            {
                activeWindow.Title = buff.ToString();
            }

            if (User32.GetWindowRect(activeWindow.Handle, out Rectangle rct))
            {
                activeWindow.Boundry = rct;
            }

            if (activeWindow.Handle == IntPtr.Zero || activeWindow.Boundry.Equals(Rectangle.Empty))
                activeWindow = ActiveWindow.Empty;

            return activeWindow;
        }

        public string GetCurrentDesktopWallpaper()
        {
            var currentWallpaper = new string('\0', MAX_PATH);
            User32.SystemParametersInfo(SPI_GETDESKWALLPAPER, currentWallpaper.Length, currentWallpaper, 0);
            return currentWallpaper.Substring(0, currentWallpaper.IndexOf('\0'));
        }
    }
}