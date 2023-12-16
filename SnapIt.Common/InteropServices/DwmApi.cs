using System.Runtime.InteropServices;
using SnapIt.Common.Entities;

namespace SnapIt.Common.InteropServices;

public class DwmApi
{
    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(nint hwnd, DWMWINDOWATTRIBUTE dwAttribute, out PInvoke.RECT pvAttribute, int cbAttribute);
}