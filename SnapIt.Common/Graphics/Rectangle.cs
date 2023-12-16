namespace SnapIt.Common.Graphics;

public class Rectangle
{
    public static Rectangle Empty
    { get { return CreateEmptyRect(); } }

    public float Left;        // x position of upper-left corner
    public float Top;         // y position of upper-left corner
    public float Right;       // x position of lower-right corner
    public float Bottom;      // y position of lower-right corner
    public Dpi Dpi;

    public Rectangle()
    {
        Dpi = Dpi.Default;
    }

    public Rectangle(float left, float top, float right, float bottom)

    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
        Dpi = Dpi.Default;
    }

    public Rectangle(float left, float top, float right, float bottom, Dpi dpi)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
        Dpi = dpi;
    }

    public float X
    { get { return Left; } }

    public float Y
    { get { return Top; } }

    public float Width
    { get { return System.Math.Abs(Right - Left); } }

    public float Height
    { get { return System.Math.Abs(Bottom - Top); } }

    public Point Center
    { get { return new Point((Left + Right) / 2, (Top + Bottom) / 2); } }

    public Point TopLeft
    { get { return new Point(Left, Top); } }

    public Point TopRight
    { get { return new Point(Right, Top); } }

    public Point BottomLeft
    { get { return new Point(Left, Bottom); } }

    public Point BottomRight
    { get { return new Point(Right, Bottom); } }

    public bool Contains(Rectangle rectangle)
    {
        return Left <= rectangle.Center.X && rectangle.Center.X <= Right && Top <= rectangle.Center.Y && rectangle.Center.Y <= Bottom;
    }

    public bool ContainsDpiAwareness(Rectangle rectangle)
    {
        var center = new Point(
            (float)(rectangle.Center.X / (rectangle.Dpi.X * Dpi.X)),
            (float)(rectangle.Center.Y / (rectangle.Dpi.Y * Dpi.Y)));

        return Left <= center.X && center.X <= Right && Top <= center.Y && center.Y <= Bottom;
    }

    public bool IsEmpty
    {
        get
        {
            return Width < 0;
        }
    }

    private static Rectangle CreateEmptyRect()
    {
        return new Rectangle(float.PositiveInfinity, float.NegativeInfinity, float.PositiveInfinity, float.NegativeInfinity);
    }

    public Rectangle GetRectangle()
    {
        return new Rectangle(Left, Top, Right, Bottom, Dpi);
    }

    public bool IntersectsWith(Rectangle rect)
    {
        if (IsEmpty || rect.IsEmpty)
        {
            return false;
        }

        return (rect.Left <= Right) &&
               (rect.Right >= Left) &&
               (rect.Top <= Bottom) &&
               (rect.Bottom >= Top);
    }

    public IList<Rectangle> GetCollisions(IList<Rectangle> rectangles)
    {
        var current = GetRectangle();
        var result = rectangles
            .Where(rectangle => current.IntersectsWith(rectangle))
            .ToList();

        return result;
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
            Rectangle r = (Rectangle)obj;
            return (Left == r.Left) && (Top == r.Top) && (Right == r.Right) && (Bottom == r.Bottom) && (Dpi.Equals(r.Dpi));
        }
    }

    public override int GetHashCode()
    {
        return (Left.GetHashCode() << 2) ^ Top.GetHashCode() ^ (Right.GetHashCode() << 2) ^ Bottom.GetHashCode() ^ Dpi.GetHashCode();
    }

    public override string ToString()
    {
        return $"X:{X}, Y:{Y}, Width:{Width}, Height:{Height}";
    }
}