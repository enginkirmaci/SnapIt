using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public int Width { get { return Math.Abs(Right - Left); } }
        public int Height { get { return Math.Abs(Bottom - Top); } }
        public Point Center { get { return new Point((Left + Right) / 2, (Top + Bottom) / 2); } }

        public static Rectangle Empty { get { return new Rectangle(); } }

        public bool Contains(Rectangle rectangle)
        {
            return Left <= rectangle.Center.X && rectangle.Center.X <= Right && Top <= rectangle.Center.Y && rectangle.Center.Y <= Bottom;
        }

        public bool ContainsDpiAwareness(Rectangle rectangle)
        {
            var center = new Point
            {
                X = (int)(rectangle.Center.X / (rectangle.Dpi.X * Dpi.X)),
                Y = (int)(rectangle.Center.Y / (rectangle.Dpi.Y * Dpi.Y))
            };

            return Left <= center.X && center.X <= Right && Top <= center.Y && center.Y <= Bottom;
        }

        public System.Windows.Rect GetRect()
        {
            return new System.Windows.Rect(
                new System.Windows.Point(Left, Top),
                new System.Windows.Point(Right, Bottom));
        }

        public IList<Rectangle> GetCollisions(IList<Rectangle> rectangles)
        {
            var rect = GetRect();

            var result = rectangles
                .Where(b => System.Windows.Rect.Intersect(rect, b.GetRect()) != System.Windows.Rect.Empty)
                .ToList();

            return result;
        }

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}, Width:{Width}, Height:{Height}";
        }
    }
}