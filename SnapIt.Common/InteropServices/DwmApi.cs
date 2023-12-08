using System.Runtime.InteropServices;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

namespace SnapIt.Common.InteropServices;

public class DwmApi
{
    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(nint hwnd, DWMWINDOWATTRIBUTE dwAttribute, out Rectangle pvAttribute, int cbAttribute);
}