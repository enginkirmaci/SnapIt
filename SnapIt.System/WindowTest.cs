using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace SnapIt
{
    public class WindowTest : Window
    {
        public WindowTest()
        {
            var screen = Screen.PrimaryScreen; // Screen.AllScreens[1];

            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Left = screen.WorkingArea.X;
            Top = screen.WorkingArea.Y;
            WindowState = WindowState.Normal;
        }

        private Border current;

        public void Close()
        {
        }

        public void Show()
        {
        }

        public Entities.Rectangle SelectElementWithPoint(int x, int y)
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

                    PresentationSource presentationsource = PresentationSource.FromVisual(this);
                    Matrix m = presentationsource.CompositionTarget.TransformToDevice;

                    double DpiWidthFactor = m.M11;
                    double DpiHeightFactor = m.M22;

                    var scaled = new Point
                    {
                        X = grid.ActualWidth * DpiWidthFactor,
                        Y = grid.ActualHeight * DpiHeightFactor
                    };

                    return new Entities.Rectangle((int)location.X, (int)location.Y, (int)scaled.X, (int)scaled.Y);
                }
            }

            return Entities.Rectangle.Empty;
        }
    }
}