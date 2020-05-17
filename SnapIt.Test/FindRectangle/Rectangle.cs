namespace SliceRectangleSample
{
    public class Rectangle
    {
        public SegmentPoint TopLeft { get; set; }
        public SegmentPoint BottomRight { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle(SegmentPoint topLeft, SegmentPoint bottomRight, int width, int height)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;
            Width = width;
            Height = height;
        }
    }
}