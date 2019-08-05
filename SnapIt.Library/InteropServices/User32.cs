using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SnapIt.Library.Entities;

namespace SnapIt.Library.InteropServices
{
	public class User32
	{
		public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern bool UnhookWindowsHookEx(int idHook);

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

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

		/////////

		[DllImport("user32.dll")]
		public static extern IntPtr GetMenu(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern int GetMenuItemCount(IntPtr hMenu);

		[DllImport("user32.dll")]
		public static extern bool DrawMenuBar(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

		public static uint MF_BYPOSITION = 0x400;
		public static uint MF_REMOVE = 0x1000;

		//////////
		[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
		private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
		private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

		// This static method is required because Win32 does not support
		// GetWindowLongPtr directly
		public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size == 8)
				return GetWindowLongPtr64(hWnd, nIndex);
			else
				return GetWindowLongPtr32(hWnd, nIndex);
		}

		[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
		private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
		private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size == 8)
				return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
			else
				return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
		}

		public const uint WS_BORDER = 0x00800000;
		public const uint WS_DLGFRAME = 0x00400000;
		public const uint WS_THICKFRAME = 0x00040000;
		public const uint WS_CAPTION = WS_BORDER | WS_DLGFRAME;
		public const uint WS_MINIMIZE = 0x20000000;
		public const uint WS_MAXIMIZE = 0x01000000;
		public const uint WS_SYSMENU = 0x00080000;
		public const uint WS_VISIBLE = 0x10000000;
		public const int GWL_STYLE = -16;

		[DllImport("user32.dll", SetLastError = false)]
		public static extern IntPtr GetDesktopWindow();

		/////////////
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

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool IsProcessDPIAware();
	}
}