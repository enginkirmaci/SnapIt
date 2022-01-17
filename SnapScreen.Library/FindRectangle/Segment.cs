using System.Drawing;
using SnapScreen.Library.Entities;

namespace SnapScreen.Library.FindRectangle
{
    internal class Segment
    {
        public Point Location { get; set; }
        public Point EndLocation { get; set; }
        public SplitDirection Orientation { get; set; }

        public override string ToString()
        {
            return $"{Location} x {EndLocation}";
        }
    }
}