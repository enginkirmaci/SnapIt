using System.Windows;
using System.Windows.Controls;

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
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}