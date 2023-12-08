using SnapIt.Common.Graphics;

namespace SnapIt.Common.Entities;

public class SnapAreaInfo
{
    public Rectangle Rectangle { get; set; }

    //public SnapWindow SnapWindow { get; set; }
    public SnapScreen Screen { get; set; }

    public static readonly SnapAreaInfo Empty = new SnapAreaInfo();
}