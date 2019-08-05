using System.Runtime.InteropServices;

namespace SnapIt.Library.Entities
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MouseHookStruct
	{
		public Point pt;
		public int hwnd;
		public int wHitTestCode;
		public int dwExtraInfo;
	}
}