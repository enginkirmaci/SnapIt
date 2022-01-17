using System;
using System.Runtime.InteropServices;
using SnapScreen.Library.Entities;

namespace SnapScreen.Library.InteropServices
{
    public class DwmApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out Rectangle pvAttribute, int cbAttribute);
    }
}