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
            var rect = this.GetRect();

            var point = new Point(
                (rect.TopLeft.X + rect.BottomRight.X) / 2,
                -10);

            var size = new Size(double.NaN, SnapControl.ActualHeight + 20);

            var newBorder = new SnapBorder();
            newBorder.SetPos(point, size, SplitDirection.Vertically);

            SnapControl.AddBorder(newBorder);
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}