using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SnapIt.Library.Entities;
using SnapIt.Library.InteropServices;

namespace SnapIt.Library.Services
{
    public class WinApiService : IWinApiService
    {
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

        public bool IsFullscreen(Rectangle activeWindowRectangle)
        {
            User32.GetWindowRect(User32.GetDesktopWindow(), out Rectangle desktopWindow);
            return activeWindowRectangle.Left == desktopWindow.Left &&
                    activeWindowRectangle.Top == desktopWindow.Top &&
                    activeWindowRectangle.Right == desktopWindow.Right &&
                    activeWindowRectangle.Bottom == desktopWindow.Bottom;
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
                SetWindowPosFlags.IgnoreZOrder | SetWindowPosFlags.ShowWindow);

            var msg = Marshal.GetLastWin32Error();
            if (msg != 0)
            {
            }
            return res;
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
    }
}