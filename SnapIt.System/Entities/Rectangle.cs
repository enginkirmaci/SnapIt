using System.Runtime.InteropServices;

namespace SnapIt.Entities
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

        public static Rectangle Empty { get { return new Rectangle(); } }
    }
}