using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace SnapIt
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            var screen = Screen.AllScreens[1];

            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Left = screen.WorkingArea.X;
            Top = screen.WorkingArea.Y;
            WindowState = WindowState.Normal;

            //MouseLeftButtonUp += Window1_MouseLeftButtonUp;
        }

        private Border current;

        public Rect SelectElementWithPoint(int x, int y)
        {
            //Visual.PointFromScreen method.
            //PresentationSource.FromVisual(this)
            if (this.IsVisible)
            {
                Point pointofScreen = new Point(x, y);

                Point Point2Window = this.PointFromScreen(pointofScreen);

                IInputElement element = this.InputHitTest(Point2Window);
                if (element != null)
                {
                    if (current != null)
                    {
                        current.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
                    }

                    var grid = current = (Border)element;

                    grid.Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));

                    var location = grid.PointToScreen(new Point(0, 0));

                    return new Rect(location, new Size(grid.ActualWidth, grid.ActualHeight));
                }
            }

            return Rect.Empty;
        }

        //private void Window1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    var control = Mouse.DirectlyOver;
        //    if (control is Grid)
        //    {
        //        var grid = (Grid)control;
        //        var height = (int)grid.ActualHeight;
        //        var width = (int)grid.ActualWidth;

        //        var location = grid.PointToScreen(new Point(0, 0));
        //        var x = (int)location.X;
        //        var y = (int)location.Y;

        //        //Debug.WriteLine($"size: {width}x{height} pos: {x}x{y}");

        //        Win32ApiTest.MoveWindow(MainWindow.ActiveWindow, x, y, width, height);
        //    }
        //}
    }
}