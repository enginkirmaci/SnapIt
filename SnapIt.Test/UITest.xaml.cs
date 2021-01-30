using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.SimpleChildWindow;
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

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.child01.SetCurrentValue(ChildWindow.IsOpenProperty, true);
        }

        private void CloseFirst_OnClick(object sender, RoutedEventArgs e)
        {
            //this.child01.Close();
        }

        private void Child01_OnClosing(object sender, CancelEventArgs e)
        {
            //e.Cancel = true; // don't close
        }
    }
}