using SnapIt.Library.Entities;
using System.Drawing;

namespace SnapIt.Library.FindRectangle
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