namespace SnapIt.Common.Graphics;

public class XComparer : IComparer<Line>
{
    public int Compare(Line x, Line y)
    {
        return x.Start.X.CompareTo(y.Start.X);
    }
}

public static class SweepLine
{
    public static IEnumerable<Rectangle> GetRectangles(IEnumerable<Line> lines)
    {
        var activeVertical = new List<Line>();

        var sweepSet = new SortedList<float, List<Line>>();

        foreach (Line oneLine in lines.Where(x => x.Start.X == x.End.X))
        {
            if (!sweepSet.ContainsKey(oneLine.Start.Y)) sweepSet.Add(oneLine.Start.Y, []);
            sweepSet[oneLine.Start.Y].Add(oneLine);

            if (!sweepSet.ContainsKey(oneLine.End.Y)) sweepSet.Add(oneLine.End.Y, []);
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
            float y = System.Math.Min(sweepSet.Keys[sweepIndex], linesHorizontal[horIndex].Start.Y);

            float verValue = linesHorizontal[horIndex].Start.Y;
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
                    float minX = activeVertical[minIndex].Start.X;
                    float maxX = activeVertical[maxIndex].Start.X;

                    foreach (Rectangle oneRec in rectangles)
                    {
                        if (minX > oneRec.TopRight.X) oneRec.TopRight.X = minX;
                        if (maxX < oneRec.BottomLeft.X) oneRec.BottomLeft.X = maxX;
                        oneRec.BottomLeft.Y = verValue;
                    }
                    completedRectangles.AddRange(rectangles);
                    //rectangles.Clear();

                    rectangles.Add(new Rectangle(activeVertical[minIndex].Start.X, verValue, activeVertical[maxIndex].Start.X, verValue));
                }
                //else rectangles.Clear();
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

        return rectangles;
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