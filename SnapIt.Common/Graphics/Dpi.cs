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

    public override bool Equals(Object obj)
    {
        // Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Dpi d = (Dpi)obj;
            return (X == d.X) && (Y == d.Y);
        }
    }

    public override int GetHashCode()
    {
        return (X.GetHashCode() << 2) ^ Y.GetHashCode();
    }
}