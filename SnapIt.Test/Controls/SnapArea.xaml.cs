using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;
using SnapIt.Test.Extensions;

namespace SnapIt.Test.Controls
{
    /// <summary>
    /// Interaction logic for SnapArea.xaml
    /// </summary>
    public partial class SnapArea : UserControl
    {
        public SnapControl SnapControl { get; set; }

        public SnapArea()
        {
            InitializeComponent();
        }

        private void SplitVertically_Click(object sender, RoutedEventArgs e)
        {
            Split(SplitDirection.Vertical);
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
            Split(SplitDirection.Horizontal);
        }

        private void Split(SplitDirection direction)
        {
            Point point;
            Size size;

            var rect = this.GetRect();

            if (direction == SplitDirection.Vertical)
            {
                point = new Point((rect.TopLeft.X + rect.BottomRight.X) / 2, rect.TopLeft.Y);
                size = new Size(double.NaN, rect.Height);
            }
            else
            {
                point = new Point(rect.TopLeft.X, (rect.TopLeft.Y + rect.BottomRight.Y) / 2);
                size = new Size(rect.Width, double.NaN);
            }

            var newBorder = new SnapBorder();
            newBorder.SetPos(point, size, direction);

            SnapControl.AddBorder(newBorder);
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}