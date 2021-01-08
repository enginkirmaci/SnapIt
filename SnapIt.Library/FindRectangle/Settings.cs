using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SnapIt.Library.Entities;

namespace SnapIt.Library.FindRectangle
{
    // Cyotek Slice Rectangle Sample
    // Copyright (c) 2013 Cyotek. All Rights Reserved.
    // http://cyotek.com
    // http://cyotek.com/blog/dividing-up-a-rectangle-based-on-pairs-of-points-using-csharp

    // If you use this code in your applications, attribution or donations are welcome.

    internal class Settings
    {
        #region Constructors

        public Settings()
        {
            Segments = new List<Segment>();
        }

        #endregion Constructors

        #region Properties

        public List<Segment> Segments { get; set; }

        public Size Size { get; set; }

        private IDictionary<Point, SegmentPoint> Points { get; set; }

        private HashSet<Rectangle> Rectangles { get; set; }

        #endregion Properties

        #region Members

        public void Calculate()
        {
            CalculatePoints();
            CalculateRectangles();
        }

        public SegmentPoint[] GetPoints()
        {
            return Points != null ? Points.Values.ToArray() : new SegmentPoint[0];
        }

        public Rectangle[] GetRectangles()
        {
            return Rectangles != null ? Rectangles.ToArray() : new Rectangle[0];
        }

        private void CalculatePoints()
        {
            List<Segment> segments;

            segments = new List<Segment>();
            Points = new Dictionary<Point, SegmentPoint>();

            //add segments representing the edges
            segments.Add(new Segment { Location = new Point(0, 0), EndLocation = new Point(Size.Width, 0), Orientation = SplitDirection.Horizontal });
            segments.Add(new Segment { Location = new Point(0, Size.Height), EndLocation = new Point(Size.Width, Size.Height), Orientation = SplitDirection.Horizontal });
            segments.Add(new Segment { Location = new Point(0, 0), EndLocation = new Point(0, Size.Height), Orientation = SplitDirection.Vertical });
            segments.Add(new Segment { Location = new Point(Size.Width, 0), EndLocation = new Point(Size.Width, Size.Height), Orientation = SplitDirection.Vertical });

            // add the rest of the segments
            segments.AddRange(Segments);

            segments.Sort((a, b) =>
            {
                int result = a.Location.X.CompareTo(b.Location.X);
                if (result == 0)
                    result = a.Location.Y.CompareTo(b.Location.Y);
                return result;
            });

            foreach (Segment segment in segments)
            {
                Segment currentSegment;

                // add the segment points
                UpdatePoint(segment.Location, segment.Orientation == SplitDirection.Horizontal ? SegmentPointConnections.Left : SegmentPointConnections.Top);
                UpdatePoint(segment.EndLocation, segment.Orientation == SplitDirection.Horizontal ? SegmentPointConnections.Right : SegmentPointConnections.Bottom);

                // calculate any intersecting points
                currentSegment = segment;
                foreach (Segment otherSegment in segments.Where(s => s != currentSegment))
                {
                    Point intersection;

                    intersection = Intersection.FindLineIntersection(segment.Location, segment.EndLocation, otherSegment.Location, otherSegment.EndLocation);
                    if (intersection != Point.Empty)
                    {
                        SegmentPointConnections flags;

                        flags = SegmentPointConnections.None;
                        if (intersection != segment.Location && intersection != segment.EndLocation)
                        {
                            if (segment.Orientation == SplitDirection.Horizontal)
                                flags |= (SegmentPointConnections.Left | SegmentPointConnections.Right);
                            else
                                flags |= (SegmentPointConnections.Top | SegmentPointConnections.Bottom);
                        }
                        else if (intersection != otherSegment.Location && intersection != otherSegment.EndLocation)
                        {
                            if (otherSegment.Orientation == SplitDirection.Horizontal)
                                flags |= (SegmentPointConnections.Left | SegmentPointConnections.Right);
                            else
                                flags |= (SegmentPointConnections.Top | SegmentPointConnections.Bottom);
                        }

                        if (flags != SegmentPointConnections.None)
                            UpdatePoint(intersection, flags);
                    }
                }
            }
        }

        private void CalculateRectangles()
        {
            SegmentPoint[] horizontalPoints;
            SegmentPoint[] verticalPoints;

            Rectangles = new HashSet<Rectangle>();
            horizontalPoints = Points.Values.OrderBy(p => p.X).ToArray();
            verticalPoints = Points.Values.OrderBy(p => p.Y).ToArray();

            foreach (SegmentPoint topLeft in Points.Values.Where(p => p.Connections.HasFlag(SegmentPointConnections.Left | SegmentPointConnections.Top)))
            {
                SegmentPoint topRight;
                SegmentPoint bottomLeft;

                topRight = horizontalPoints.FirstOrDefault(p => p.X > topLeft.X && p.Y == topLeft.Y && p.Connections.HasFlag(SegmentPointConnections.Right | SegmentPointConnections.Top));
                bottomLeft = verticalPoints.FirstOrDefault(p => p.X == topLeft.X && p.Y > topLeft.Y && p.Connections.HasFlag(SegmentPointConnections.Left | SegmentPointConnections.Bottom));

                if (topRight != null && bottomLeft != null)
                {
                    SegmentPoint bottomRight;

                    bottomRight = horizontalPoints.FirstOrDefault(p => p.X == topRight.X && p.Y == bottomLeft.Y && p.Connections.HasFlag(SegmentPointConnections.Right | SegmentPointConnections.Bottom));

                    if (bottomRight != null)
                    {
                        Rectangle rectangle;

                        rectangle = new Rectangle(topLeft, bottomRight, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

                        Rectangles.Add(rectangle);
                    }
                }
            }
        }

        private void UpdatePoint(Point location, SegmentPointConnections connections)
        {
            SegmentPoint point;

            if (!Points.TryGetValue(location, out point))
            {
                point = new SegmentPoint { Location = location, Connections = connections };
                Points.Add(point.Location, point);
            }
            else if (!point.Connections.HasFlag(connections))
                point.Connections |= connections;
        }

        #endregion Members
    }
}