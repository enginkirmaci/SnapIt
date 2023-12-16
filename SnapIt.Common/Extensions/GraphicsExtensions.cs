using System.Windows;
using System.Windows.Media;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace SnapIt.Common.Extensions;

public static class GraphicsExtensions
{
    public static Color Convert(this Graphics.Color color)
    {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    public static Graphics.Color Convert(this Color color)
    {
        return new Graphics.Color(color.A, color.R, color.G, color.B);
    }

    public static Point Convert(this Graphics.Point point)
    {
        return new Point(point.X, point.Y);
    }

    public static Graphics.Point Convert(this Point point)
    {
        return new Graphics.Point((float)point.X, (float)point.Y);
    }

    public static Rect Convert(this Graphics.Rectangle rectangle)
    {
        return new Rect(rectangle.TopLeft.Convert(), rectangle.BottomRight.Convert());
    }

    public static Graphics.Rectangle Convert(this Rect rectangle)
    {
        return new Graphics.Rectangle((float)rectangle.Left, (float)rectangle.Top, (float)rectangle.Right, (float)rectangle.Bottom);
    }

    public static Size Convert(this Graphics.Size size)
    {
        return new Size(size.Width, size.Height);
    }

    public static Graphics.Size Convert(this Size size)
    {
        return new Graphics.Size((float)size.Width, (float)size.Height);
    }
}