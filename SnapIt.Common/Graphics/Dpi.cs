namespace SnapIt.Common.Graphics;

public class Dpi
{
    public float X;
    public float Y;

    public static Dpi Default
    { get { return new Dpi { X = 1.0f, Y = 1.0f }; } }

    public override string ToString()
    {
        return $"X:{X}, Y:{Y}";
    }
}