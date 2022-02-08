using System.Windows;

namespace SnapIt.Library.Entities
{
    public class LayoutOverlay
    {
        public Point Point { get; set; }
        public Size Size { get; set; }
        public LayoutOverlay MiniOverlay { get; set; }
    }
}