using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapIt.Test.Library
{
    public class XComparer : IComparer<Line>
    {
        public int Compare(Line x, Line y)
        {
            return x.Start.X.CompareTo(y.Start.X);
        }
    }

    public class SweepLine
    {
        public static List<Rectangle> GetRectangles(List<Line> lines)
        {
            //List<Line> lines = new List<Line>();
            //lines.Add(new Line() { Start = new Point(0.5, 12.5), End = new Point(10, 12.5) });
            //lines.Add(new Line() { Start = new Point(2.5, 9.5), End = new Point(15.8, 9.5) });
            //lines.Add(new Line() { Start = new Point(6, 8.5), End = new Point(16.3, 8.5) });
            //lines.Add(new Line() { Start = new Point(3.5, 8.5), End = new Point(3.5, 12.5) });
            //lines.Add(new Line() { Start = new Point(7, 4.2), End = new Point(7, 13.8) });
            //lines.Add(new Line() { Start = new Point(10, 5.8), End = new Point(10, 14.2) });
            //lines.Add(new Line() { Start = new Point(15.6, 0), End = new Point(15.6, 16) });
            //lines.Add(new Line() { Start = new Point(1.6, 20), End = new Point(15.6, 20) });

            var activeVertical = new List<Line>();

            SortedList<double, List<Line>> sweepSet = new SortedList<double, List<Line>>();

            foreach (Line oneLine in lines.Where(x => x.Start.X == x.End.X))
            {
                if (!sweepSet.ContainsKey(oneLine.Start.Y)) sweepSet.Add(oneLine.Start.Y, new List<Line>());
                sweepSet[oneLine.Start.Y].Add(oneLine);

                if (!sweepSet.ContainsKey(oneLine.End.Y)) sweepSet.Add(oneLine.End.Y, new List<Line>());
                sweepSet[oneLine.End.Y].Add(oneLine);
            }

            var linesHorizontal = lines.Where(x => x.Start.Y == x.End.Y).OrderBy(x => x.Start.Y).ToList();

            List<Rectangle> rectangles = new List<Rectangle>();
            List<Rectangle> completedRectangles = new List<Rectangle>();
            var xComp = new XComparer();

            int horIndex = 0;
            int sweepIndex = 0;
            while (sweepIndex < sweepSet.Count)
            {
                double y = Math.Min(sweepSet.Keys[sweepIndex], linesHorizontal[horIndex].Start.Y);

                double verValue = linesHorizontal[horIndex].Start.Y;
                //add lines which are influencing
                if (sweepSet.ContainsKey(y))
                {
                    foreach (Line oneLine in sweepSet[y].Where(x => x.Start.Y == y))
                    {
                        int index = activeVertical.BinarySearch(oneLine, xComp);
                        if (index < 0) index = ~index;
                        activeVertical.Insert(index, oneLine);
                    }
                }
                if (y == verValue)
                {
                    int minIndex = GetMinIndex(activeVertical, linesHorizontal[horIndex]);
                    int maxIndex = GetMaxIndex(activeVertical, linesHorizontal[horIndex]);

                    if (minIndex != maxIndex && minIndex < activeVertical.Count && maxIndex < activeVertical.Count)
                    {
                        double minX = activeVertical[minIndex].Start.X;
                        double maxX = activeVertical[maxIndex].Start.X;

                        foreach (Rectangle oneRec in rectangles)
                        {
                            if (minX > oneRec.TopRight.X) oneRec.TopRight.X = minX;
                            if (maxX < oneRec.BottomLeft.X) oneRec.BottomLeft.X = maxX;
                            oneRec.BottomLeft.Y = verValue;
                        }
                        completedRectangles.AddRange(rectangles);
                        rectangles.Clear();

                        rectangles.Add(new Rectangle(new Point(activeVertical[minIndex].Start.X, verValue), new Point(activeVertical[maxIndex].Start.X, verValue)));
                    }
                    else rectangles.Clear();
                }
                //Cleanup lines which end
                if (sweepSet.ContainsKey(y))
                {
                    foreach (Line oneLine in sweepSet[y].Where(x => x.End.Y == y))
                    {
                        activeVertical.Remove(oneLine);
                    }
                }

                if (y >= verValue)
                {
                    horIndex++;
                    if (horIndex == linesHorizontal.Count) break;
                    if (y == sweepSet.Keys[sweepIndex]) sweepIndex++;
                }
                else
                {
                    sweepIndex++;
                }
            }

            return completedRectangles;
        }

        private static int GetMinIndex(List<Line> Lines, Line Horizontal)
        {
            var xComp = new XComparer();
            int minIndex = Lines.BinarySearch(Horizontal, xComp);
            if (minIndex < 0) minIndex = ~minIndex;
            return minIndex;
        }

        private static int GetMaxIndex(List<Line> Lines, Line Horizontal)
        {
            var xComp = new XComparer();
            int maxIndex = Lines.BinarySearch(new Line() { Start = Horizontal.End }, xComp);
            if (maxIndex < 0) maxIndex = ~maxIndex - 1;
            return maxIndex;
        }
    }

    public class Point
    {
        public Point(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }

    public class Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        public override string ToString()
        {
            return $"{Start.X}, {Start.Y}   {End.X}, {End.Y} ";
        }
    }

    public class Rectangle
    {
        public Rectangle()
        { }

        public Rectangle(Point TopRight, Point BottomLeft)
        {
            this.TopRight = TopRight;
            this.BottomLeft = BottomLeft;
        }

        public Point TopRight { get; set; }
        public Point BottomLeft { get; set; }
    }
}