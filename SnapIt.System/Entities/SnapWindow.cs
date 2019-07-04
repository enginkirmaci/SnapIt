using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using SnapIt.InteropServices;

namespace SnapIt.Entities
{
    public class SnapWindow : Window
    {
        private Border current;
        private Grid mainGrid;
        private Screen screen;
        private double DpiX = 1.0;
        private double DpiY = 1.0;

        public SnapWindow(Screen screen)
        {
            this.screen = screen;

            var pixelToDpiX = 1.0;
            var pixelToDpiY = 1.0;

            CalculateDpi();

            //TODO use it wisely
            var isDPIAware = User32.IsProcessDPIAware();

            //var screenInfo = User32Test.GetScreenInfo(screen.DeviceName);
            //pixelToDpiX = screen.Bounds.Width / (double)screenInfo.dmPelsWidth;
            //pixelToDpiY = screen.Bounds.Height / (double)screenInfo.dmPelsHeight;

            Topmost = true;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Colors.Transparent);
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            Width = screen.WorkingArea.Width * pixelToDpiX;
            Height = screen.WorkingArea.Height * pixelToDpiY;
            Top = screen.WorkingArea.Y * pixelToDpiX;
            Left = screen.WorkingArea.X * pixelToDpiX;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
        }

        private void CalculateDpi()
        {
            var screenInfo = User32Test.GetScreenInfo(screen.DeviceName);
            DpiX = screen.Bounds.Width / (double)screenInfo.dmPelsWidth;
            DpiY = screen.Bounds.Height / (double)screenInfo.dmPelsHeight;
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
            if (IsVisible)
            {
                System.Windows.Point point = new System.Windows.Point(x, y);
                var Point2Window = PointFromScreen(point);
                Point2Window.X *= DpiX;
                Point2Window.Y *= DpiY;

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

                    System.Windows.Point scaled = grid.PointToScreen(new System.Windows.Point(grid.ActualWidth, grid.ActualHeight));

                    var res = new Rectangle(
                        (int)(location.X / DpiX),
                        (int)(location.Y / DpiY),
                       (int)(scaled.X / DpiX),
                        (int)(scaled.Y / DpiY));

                    Debug.WriteLine(DpiX + " " + point + " -> " + location + " " + scaled + " " + res);

                    return res;
                }
                else
                {
                    //TODO imporove here. mooving on different screens, old one preserves the hover style
                    if (current != null)
                    {
                        current.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
                    }
                }
            }

            return Rectangle.Empty;
        }
    }
}