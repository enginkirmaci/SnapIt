using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SnapIt.Library.Entities;
using SnapIt.Library.InteropServices;

namespace SnapIt.Library
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

		public static IEnumerable<string> GetOpenWindowsNames()
		{
			return GetOpenWindows().Select(i => i.Value).Distinct().OrderBy(i => i);
		}

		public static bool IsFullscreen(Rectangle activeWindowRectangle)
		{
			GetWindowRect(GetDesktopWindow(), out Rectangle desktopWindow);
			return activeWindowRectangle.Left == desktopWindow.Left &&
					activeWindowRectangle.Top == desktopWindow.Top &&
					activeWindowRectangle.Right == desktopWindow.Right &&
					activeWindowRectangle.Bottom == desktopWindow.Bottom;
		}

		public static void MoveWindow(ActiveWindow activeWindow, Rectangle newRect)
		{
			MoveWindow(activeWindow, newRect.X, newRect.Y, newRect.Width, newRect.Height);
		}

		public static bool MoveWindow(ActiveWindow activeWindow, int X, int Y, int width, int height)
		{
			var res = SetWindowPos(
				activeWindow.Handle,
				(HWND)SpecialWindowHandles.HWND_TOP,
				X,
				Y,
				width,
				height,
				SetWindowPosFlags.IgnoreZOrder);

			var msg = Marshal.GetLastWin32Error();
			if (msg != 0)
			{
			}
			return res;
		}

		public static void GetWindowMargin(ActiveWindow activeWindow, out Rectangle withMargin)
		{
			DwmApi.DwmGetWindowAttribute(
							activeWindow.Handle,
							DWMWINDOWATTRIBUTE.ExtendedFrameBounds,
							out withMargin,
							Marshal.SizeOf(typeof(Rectangle)));
		}

		public static ActiveWindow GetActiveWindow()
		{
			//Code to write inside timer event or some other event
			int chars = 256;
			StringBuilder buff = new StringBuilder(chars);

			//This method will return a handle for the active window on desktop
			HWND handle = GetForegroundWindow();

			//This method will give the window text on title bar of the active window
			if (GetWindowText(handle, buff, chars) > 0)
			{
				//Debug.WriteLine(buff.ToString());

				return new ActiveWindow
				{
					Handle = handle,
					Title = buff.ToString()
				};
			}

			return ActiveWindow.Empty;
		}

		public static Rectangle GetWindowRectangle(ActiveWindow activeWindow)
		{
			if (!GetWindowRect(activeWindow.Handle, out Rectangle rct))
			{
				return new Rectangle();
			}

			return rct;
		}

		public static DEVMODE GetScreenInfo(string deviceName)
		{
			const int ENUM_CURRENT_SETTINGS = -1;

			var dm = new DEVMODE
			{
				dmSize = (short)Marshal.SizeOf(typeof(DEVMODE))
			};

			EnumDisplaySettings(deviceName, ENUM_CURRENT_SETTINGS, ref dm);

			return dm;
		}
	}
}