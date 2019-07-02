using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace SnapIt.Entities
{
    public class SnapWindow : Window
    {
        private Border current;
        private Grid mainGrid;

        public SnapWindow()
        {
            var screen = Screen.PrimaryScreen; // Screen.AllScreens[1];

            Topmost = true;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Colors.Transparent);
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Top = screen.WorkingArea.Y;
            Left = screen.WorkingArea.X;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
        }

        public void CreateGrids()
        {
            mainGrid = new Grid();

            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());

            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    var grid = new Grid();
                    grid.Children.Add(AddSnapArea());

                    mainGrid.Children.Add(grid);

                    Grid.SetColumn(grid, i);
                    Grid.SetRow(grid, j);
                }

            Content = mainGrid;
        }

        public Border AddSnapArea()
        {
            var border = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100)),
                BorderThickness = new Thickness(1, 1, 1, 1)
            };

            return border;
        }

        public Rectangle SelectElementWithPoint(int x, int y)
        {
            //Visual.PointFromScreen method.
            //PresentationSource.FromVisual(this)
            if (IsVisible)
            {
                var Point2Window = PointFromScreen(new System.Windows.Point(x, y));

                var element = InputHitTest(Point2Window);
                if (element != null)
                {
                    if (current != null)
                    {
                        current.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
                    }

                    var grid = current = (Border)element;

                    grid.Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));

                    System.Windows.Point location = grid.PointToScreen(new System.Windows.Point(0, 0));

                    PresentationSource presentationsource = PresentationSource.FromVisual(this);
                    Matrix m = presentationsource.CompositionTarget.TransformToDevice;

                    double DpiWidthFactor = m.M11;
                    double DpiHeightFactor = m.M22;

                    var scaled = new System.Windows.Point
                    {
                        X = grid.ActualWidth * DpiWidthFactor,
                        Y = grid.ActualHeight * DpiHeightFactor
                    };

                    return new Rectangle((int)location.X, (int)location.Y, (int)location.X + (int)scaled.X, (int)location.Y + (int)scaled.Y);
                }
            }

            return Rectangle.Empty;
        }
    }
}