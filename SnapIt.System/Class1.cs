using PInvoke;
using System;
using System.Diagnostics;

namespace SnapIt.System
{
    public class Class1
    {
        public static void Initialize()
        {
        }

        public static bool MoveWindow(IntPtr hWnd, int X, int Y, int width, int height)
        {
            var res = User32.SetWindowPos(
                hWnd,
                User32.SpecialWindowHandles.HWND_TOP,
                X,
                Y,
                width,
                height,
                User32.SetWindowPosFlags.SWP_NOZORDER);

            var msg = Kernel32.GetLastError();
            if (msg != 0)
            {
                Debug.WriteLine("error: " + msg);
            }
            return res;
        }
    }
}