namespace SnapIt.Common.Graphics;

public class Size
{
    public static Size Empty = new(0, 0);
    public float Width { get; set; }
    public float Height { get; set; }

    public Size()
    { }

    public Size(float width, float height)
    {
        Width = width;
        Height = height;
    }
}