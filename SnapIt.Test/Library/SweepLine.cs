using System;
using System.Collections.Generic;
using System.Linq;

namespace SnapIt.Test.Library
{
    public class XComparer : IComparer<SnapLine>
    {
        public int Compare(SnapLine x, SnapLine y)
        {
            return x.Start.X.CompareTo(y.Start.X);
        }
    }

    public static class SweepLine
    {
        public static IEnumerable<Rectangle> GetRectangles(IEnumerable<SnapLine> lines)
        {
            var activeVertical = new List<SnapLine>();

            var sweepSet = new SortedList<double, List<SnapLine>>();

            foreach (SnapLine oneLine in lines.Where(x => x.Start.X == x.End.X))
            {
                if (!sweepSet.ContainsKey(oneLine.Start.Y)) sweepSet.Add(oneLine.Start.Y, new List<SnapLine>());
                sweepSet[oneLine.Start.Y].Add(oneLine);

                if (!sweepSet.ContainsKey(oneLine.End.Y)) sweepSet.Add(oneLine.End.Y, new List<SnapLine>());
                sweepSet[oneLine.End.Y].Add(oneLine);
            }

            var linesHorizontal = lines.Where(x => x.Start.Y == x.End.Y).OrderBy(x => x.Start.Y).ToList();

            var rectangles = new List<Rectangle>();
            var completedRectangles = new List<Rectangle>();
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
                    foreach (SnapLine oneLine in sweepSet[y].Where(x => x.Start.Y == y))
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

                        rectangles.Add(new Rectangle(new SnapPoint(activeVertical[minIndex].Start.X, verValue), new SnapPoint(activeVertical[maxIndex].Start.X, verValue)));
                    }
                    else rectangles.Clear();
                }
                //Cleanup lines which end
                if (sweepSet.ContainsKey(y))
                {
                    foreach (SnapLine oneLine in sweepSet[y].Where(x => x.End.Y == y))
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

        private static int GetMinIndex(List<SnapLine> Lines, SnapLine Horizontal)
        {
            var xComp = new XComparer();
            int minIndex = Lines.BinarySearch(Horizontal, xComp);
            if (minIndex < 0) minIndex = ~minIndex;
            return minIndex;
        }

        private static int GetMaxIndex(List<SnapLine> Lines, SnapLine Horizontal)
        {
            var xComp = new XComparer();
            int maxIndex = Lines.BinarySearch(new SnapLine() { Start = Horizontal.End }, xComp);
            if (maxIndex < 0) maxIndex = ~maxIndex - 1;
            return maxIndex;
        }
    }

    public class SnapPoint
    {
        public SnapPoint(double X, double Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }

    public class SnapLine
    {
        public SnapPoint Start { get; set; }
        public SnapPoint End { get; set; }

        public override string ToString()
        {
            return $"{Start.X}, {Start.Y}   {End.X}, {End.Y} ";
        }
    }

    public class Rectangle
    {
        public Rectangle()
        { }

        public Rectangle(SnapPoint TopRight, SnapPoint BottomLeft)
        {
            this.TopRight = TopRight;
            this.BottomLeft = BottomLeft;
        }

        public SnapPoint TopRight { get; set; }
        public SnapPoint BottomLeft { get; set; }
    }
}