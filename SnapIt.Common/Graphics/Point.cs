namespace SnapIt.Common.Graphics;

public class Point
{
    public static Point Empty = new(0, 0);

    public float X { get; set; }
    public float Y { get; set; }

    public Point()
    { }

    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }
}