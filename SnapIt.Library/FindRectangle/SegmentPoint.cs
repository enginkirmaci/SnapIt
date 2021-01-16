using System.Drawing;

namespace SnapIt.Library.FindRectangle
{
    public class SegmentPoint
    {
        public SegmentPointConnections Connections { get; set; }

        public Point Location { get; set; }

        public int X
        {
            get { return Location.X; }
        }

        public int Y
        {
            get { return Location.Y; }
        }
    }
}