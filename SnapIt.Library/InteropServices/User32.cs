using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SnapIt.Library.Entities;

namespace SnapIt.Library.InteropServices
{
	public class User32
	{
		[DllImport("user32.dll")]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetShellWindow();

		public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

		[DllImport("user32.dll")]
		public static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

		[DllImport("user32.dll", SetLastError = false)]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll")]
		public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

		[StructLayout(LayoutKind.Sequential)]
		public struct DEVMODE
		{
			private const int CCHDEVICENAME = 0x20;
			private const int CCHFORMNAME = 0x20;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string dmDeviceName;

			public short dmSpecVersion;
			public short dmDriverVersion;
			public short dmSize;
			public short dmDriverExtra;
			public int dmFields;
			public int dmPositionX;
			public int dmPositionY;
			public ScreenOrientation dmDisplayOrientation;
			public int dmDisplayFixedOutput;
			public short dmColor;
			public short dmDuplex;
			public short dmYResolution;
			public short dmTTOption;
			public short dmCollate;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string dmFormName;

			public short dmLogPixels;
			public int dmBitsPerPel;
			public int dmPelsWidth;
			public int dmPelsHeight;
			public int dmDisplayFlags;
			public int dmDisplayFrequency;
			public int dmICMMethod;
			public int dmICMIntent;
			public int dmMediaType;
			public int dmDitherType;
			public int dmReserved1;
			public int dmReserved2;
			public int dmPanningWidth;
			public int dmPanningHeight;
		}
	}
}