using System.Drawing;
using SnapIt.Library.Entities;

namespace SnapIt.Library.FindRectangle
{
    internal class Segment
    {
        public Point Location { get; set; }
        public Point EndLocation { get; set; }
        public SplitDirection Orientation { get; set; }
    }
}