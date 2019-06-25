using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SnapIt
{
    using System.Windows;
    using static SnapIt.Win32Api;
    using HWND = IntPtr;

    public static class Win32ApiTest
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

        public static void MoveWindow(HWND activeWindow, Rect newRect)
        {
            MoveWindow(activeWindow, (int)newRect.X, (int)newRect.Y, (int)newRect.Width, (int)newRect.Height);
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

        public static RECT GetWindowRectangle(HWND hWnd)
        {
            if (!GetWindowRect(hWnd, out RECT rct))
            {
                Debug.WriteLine("ERROR");
                return new RECT();
            }

            return rct;
        }

        public static void RemoveMenuBar(HWND hWnd)
        {
            //get menu
            HWND HMENU = GetMenu(hWnd);
            //get item count
            int count = GetMenuItemCount(HMENU);
            //loop & remove
            for (int i = 0; i < count; i++)
                RemoveMenu(HMENU, 0, (MF_BYPOSITION | MF_REMOVE));
        }

        public static void DrawMenu(HWND hWnd)
        {
            //force a redraw
            DrawMenuBar(hWnd);
        }

        public static HWND RemoveBorders(HWND WindowHandle)
        {
            var MenuHandle = GetMenu(WindowHandle);
            int count = GetMenuItemCount(MenuHandle);
            for (int i = 0; i < count; i++)
                RemoveMenu(MenuHandle, 0, (0x40 | 0x10));

            int WindowStyle = GetWindowLongPtr(WindowHandle, -16);

            //Redraw
            DrawMenuBar(WindowHandle);
            SetWindowLongPtr(WindowHandle, -16, (WindowStyle & ~0x00080000));
            SetWindowLongPtr(WindowHandle, -16, (WindowStyle & ~0x00800000 | 0x00400000));
            return MenuHandle;
        }
    }
}