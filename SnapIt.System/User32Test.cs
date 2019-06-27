using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SnapIt.Entities;

namespace SnapIt
{
    using static InteropServices.User32;
    using HWND = IntPtr;

    public static class User32Test
    {
        public static IDictionary<HWND, string> GetOpenWindows()
        {
            HWND shellWindow = GetShellWindow();
            Dictionary<HWND, string> windows = new Dictionary<HWND, string>();

            EnumWindows(delegate (HWND hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;
            }, 0);

            return windows;
        }

        public static void MoveWindow(HWND activeWindow, Rectangle newRect)
        {
            MoveWindow(activeWindow, newRect.X, newRect.Y, newRect.Width, newRect.Height);
        }

        public static bool MoveWindow(HWND hWnd, int X, int Y, int width, int height)
        {
            var res = SetWindowPos(
                hWnd,
                (HWND)SpecialWindowHandles.HWND_TOP,
                X,
                Y,
                width,
                height,
                SetWindowPosFlags.IgnoreZOrder);

            var msg = Marshal.GetLastWin32Error();
            if (msg != 0)
            {
                Debug.WriteLine("error: " + msg);
            }
            return res;
        }

        public static HWND GetActiveWindow()
        {
            //Code to write inside timer event or some other event
            int chars = 256;
            StringBuilder buff = new StringBuilder(chars);

            //This method will return a handle for the active window on desktop
            HWND handle = GetForegroundWindow();

            //This method will give the window text on title bar of the active window
            if (GetWindowText(handle, buff, chars) > 0)
            {
                Debug.WriteLine(buff.ToString().ToLower());

                return handle;
            }

            return HWND.Zero;
        }

        public static Rectangle GetWindowRectangle(HWND hWnd)
        {
            if (!GetWindowRect(hWnd, out Rectangle rct))
            {
                Debug.WriteLine("ERROR");
                return new Rectangle();
            }

            return rct;
        }

        public static uint RemoveBorders(HWND hWnd)
        {
            var MenuHandle = GetMenu(hWnd);
            int count = GetMenuItemCount(MenuHandle);
            for (int i = 0; i < count; i++)
                RemoveMenu(MenuHandle, 0, (0x40 | 0x10));

            var windowStyle = (uint)GetWindowLongPtr(hWnd, GWL_STYLE);

            var currentstyle = windowStyle;
            uint[] styles = new uint[] { WS_CAPTION, WS_THICKFRAME, WS_MINIMIZE, WS_MAXIMIZE, WS_SYSMENU };

            foreach (uint style in styles)
            {
                if ((currentstyle & style) != 0)
                {
                    currentstyle &= ~style;
                }
            }

            SetWindowLongPtr(hWnd, GWL_STYLE, (HWND)currentstyle);

            return windowStyle;
        }

        public static void RedrawBorders(HWND hWnd, uint windowStyle)
        {
            DrawMenuBar(hWnd);
            SetWindowLongPtr(hWnd, GWL_STYLE, (HWND)windowStyle);
        }
    }
}