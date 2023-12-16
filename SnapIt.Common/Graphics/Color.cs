namespace SnapIt.Common.Graphics;

public class Color
{
    public static Color Empty = new(0, 0, 0, 0);

    public byte R { get; set; }

    public byte G { get; set; }

    public byte B { get; set; }

    public byte A { get; set; }

    public Color()
    { }

    public Color(byte a, byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
}