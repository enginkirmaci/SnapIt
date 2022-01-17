using SnapScreen.Library.Controls;

namespace SnapScreen.Library.Entities
{
    public class SnapAreaInfo
    {
        public Rectangle Rectangle { get; set; }
        public SnapWindow SnapWindow { get; set; }

        public static readonly SnapAreaInfo Empty = new SnapAreaInfo();
    }
}