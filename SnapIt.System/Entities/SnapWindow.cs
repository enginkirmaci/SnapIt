using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using SnapIt.Extensions;

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

            CalculateDpi();

            Topmost = true;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Colors.Transparent);
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Left = screen.WorkingArea.X;
            Top = screen.WorkingArea.Y;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var wih = new WindowInteropHelper(this);
            IntPtr hWnd = wih.Handle;

            User32Test.MoveWindow(hWnd, screen.WorkingArea.Left, screen.WorkingArea.Top, screen.WorkingArea.Width, screen.WorkingArea.Height);
        }

        private void CalculateDpi()
        {
            screen.GetDpi(DpiType.Effective, out uint x, out uint y);

            DpiX = 96.0 / x;
            DpiY = 96.0 / y;
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

                    var windowRectangle = new Rectangle(
                       (int)location.X,
                       (int)location.Y,
                       (int)scaled.X,
                       (int)scaled.Y);

                    return windowRectangle;
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