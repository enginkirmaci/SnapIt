using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SnapIt.Library.Extensions
{
	public static class ScreenExtension
	{
		public static void GetDpi(this Screen screen, DpiType dpiType, out uint dpiX, out uint dpiY)
		{
			var pnt = new Point(screen.Bounds.Left + 1, screen.Bounds.Top + 1);
			var mon = MonitorFromPoint(pnt, 2/*MONITOR_DEFAULTTONEAREST*/);
			GetDpiForMonitor(mon, dpiType, out dpiX, out dpiY);
		}

		//https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
		[DllImport("User32.dll")]
		private static extern IntPtr MonitorFromPoint([In]Point pt, [In]uint dwFlags);

		//https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
		[DllImport("Shcore.dll")]
		private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);
	}

	//https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
	public enum DpiType
	{
		Effective = 0,
		Angular = 1,
		Raw = 2,
	}
}