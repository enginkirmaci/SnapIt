using SnapIt.Library.Entities;
using System;
using System.Runtime.InteropServices;

namespace SnapIt.Library.InteropServices
{
    public class DwmApi
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out Rectangle pvAttribute, int cbAttribute);
    }
}