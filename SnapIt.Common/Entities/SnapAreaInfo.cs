using SnapIt.Common.Graphics;

namespace SnapIt.Common.Entities;

public class SnapAreaInfo
{
    public Rectangle Rectangle { get; set; }

    public ActiveWindow ActiveWindow { get; set; }
    public SnapScreen Screen { get; set; }

    public static readonly SnapAreaInfo Empty = new SnapAreaInfo();
}