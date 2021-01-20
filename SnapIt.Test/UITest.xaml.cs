using System.Windows.Input;

namespace SnapIt.Test
{
    /// <summary>
    /// Interaction logic for UITest.xaml
    /// </summary>
    public partial class UITest
    {
        public UITest()
        {
            InitializeComponent();
        }

        private void ToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}