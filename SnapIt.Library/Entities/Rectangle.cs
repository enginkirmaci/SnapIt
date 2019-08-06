using System.Runtime.InteropServices;

namespace SnapIt.Library.Entities
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Rectangle
	{
		public int Left;        // x position of upper-left corner
		public int Top;         // y position of upper-left corner
		public int Right;       // x position of lower-right corner
		public int Bottom;      // y position of lower-right corner

		public Rectangle(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public int X { get { return Left; } }
		public int Y { get { return Top; } }
		public int Width { get { return Right - Left; } }
		public int Height { get { return Bottom - Top; } }

		public bool Contains(Rectangle rectangle)
		{
			return Left <= rectangle.Left && rectangle.Left < Right && Top <= rectangle.Top && rectangle.Top < Bottom;
		}

		public static Rectangle Empty { get { return new Rectangle(); } }

		public override string ToString()
		{
			return $"X:{X}, Y:{Y}, Width:{Width}, Height:{Height}";
		}
	}
}