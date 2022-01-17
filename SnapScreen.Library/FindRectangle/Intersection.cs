using System.Drawing;

namespace SnapScreen.Library.FindRectangle
{
    internal static class Intersection
    {
        public static Point FindLineIntersection(Point start1, Point end1, Point start2, Point end2)
        {
            int denom;
            int x;
            int y;

            // http://stackoverflow.com/a/1120126/148962

            denom = ((end1.X - start1.X) * (end2.Y - start2.Y)) - ((end1.Y - start1.Y) * (end2.X - start2.X));
            x = 0;
            y = 0;

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (denom != 0) // ReSharper restore CompareOfFloatsByEqualityOperator
            {
                int numer;
                int r;
                int numer2;
                int s;

                numer = ((start1.Y - start2.Y) * (end2.X - start2.X)) - ((start1.X - start2.X) * (end2.Y - start2.Y));
                r = numer / denom;
                numer2 = ((start1.Y - start2.Y) * (end1.X - start1.X)) - ((start1.X - start2.X) * (end1.Y - start1.Y));
                s = numer2 / denom;

                if (r >= 0 && r <= 1 && s >= 0 && s <= 1)
                {
                    x = (start1.X + (r * (end1.X - start1.X)));
                    y = (start1.Y + (r * (end1.Y - start1.Y)));
                }
            }

            return new Point(x, y);
        }
    }
}