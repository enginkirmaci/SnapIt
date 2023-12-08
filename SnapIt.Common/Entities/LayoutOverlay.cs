using SnapIt.Common.Graphics;

namespace SnapIt.Common.Entities;

public class LayoutOverlay
{
    public Point Point { get; set; } = Point.Empty;
    public Size Size { get; set; } = Size.Empty;
    public LayoutOverlay MiniOverlay { get; set; }
}