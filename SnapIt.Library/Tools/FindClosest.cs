using SnapIt.Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapIt.Library.Tools
{
    public class FindClosest
    {
        public static Rectangle? GetClosestRectangle(IList<Rectangle> rectangles, Rectangle current, MoveDirection direction)
        {
            IEnumerable<Rectangle> rectangleInDirection = new List<Rectangle>();
            Rectangle directionRectangle = Rectangle.Empty;

            switch (direction)
            {
                case MoveDirection.Up:
                    directionRectangle = new Rectangle
                    {
                        Top = int.MinValue,
                        Bottom = current.Bottom - 10,
                        Left = current.Left + 10,
                        Right = current.Right - 10,
                    };

                    break;

                case MoveDirection.Down:
                    directionRectangle = new Rectangle
                    {
                        Top = current.Top + 10,
                        Bottom = int.MaxValue,
                        Left = current.Left + 10,
                        Right = current.Right - 10,
                    };

                    break;

                case MoveDirection.Left:
                    directionRectangle = new Rectangle
                    {
                        Top = current.Top + 10,
                        Bottom = current.Bottom - 10,
                        Left = int.MinValue,
                        Right = current.Right - 10,
                    };

                    break;

                case MoveDirection.Right:
                    directionRectangle = new Rectangle
                    {
                        Top = current.Top + 10,
                        Bottom = current.Bottom - 10,
                        Left = current.Left + 10,
                        Right = int.MaxValue,
                    };

                    break;
            }

            rectangles.Remove(current);
            rectangleInDirection = directionRectangle.GetCollisions(rectangles);

            return rectangleInDirection
                .Select(rectangle => new { distance = GetDistance(rectangle.Center, current.Center), rectangle })
                .OrderBy(rectangle => rectangle.distance)
                .FirstOrDefault()?.rectangle;
        }

        private static double GetDistance(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}