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
        public Dpi Dpi;

        public Rectangle(int left, int top, int right, int bottom)

        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            Dpi = Dpi.Default;
        }

        public Rectangle(int left, int top, int right, int bottom, Dpi dpi)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            Dpi = dpi;
        }

        public int X { get { return Left; } }
        public int Y { get { return Top; } }
        public int Width { get { return Right - Left; } }
        public int Height { get { return Bottom - Top; } }

        public static Rectangle Empty { get { return new Rectangle(); } }

        public bool Contains(Rectangle rectangle)
        {
            return Left <= rectangle.Left && rectangle.Left < Right && Top <= rectangle.Top && rectangle.Top < Bottom;
        }

        public bool ContainsDpiAwareness(Rectangle rectangle)
        {
            var temp = new Rectangle(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom, rectangle.Dpi);

            temp.Left = (int)(temp.Left / (temp.Dpi.X * Dpi.X));
            temp.Right = (int)(temp.Right / (temp.Dpi.X * Dpi.X));

            temp.Top = (int)(temp.Top / (temp.Dpi.Y * Dpi.Y));
            temp.Bottom = (int)(temp.Bottom / (temp.Dpi.Y * Dpi.Y));

            return Left <= temp.Left && temp.Left < Right && Top <= temp.Top && temp.Top < Bottom;
        }

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}, Width:{Width}, Height:{Height}";
        }
    }
}