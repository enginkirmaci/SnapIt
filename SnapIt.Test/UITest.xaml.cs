using System.Windows.Input;
using SnapIt.Test.DesignTime;

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

            DataContext = new MainWindowDesignViewModel();
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