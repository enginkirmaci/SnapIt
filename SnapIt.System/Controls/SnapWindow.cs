using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using SnapIt.Entities;
using SnapIt.Extensions;
using Point = System.Windows.Point;

namespace SnapIt.Controls
{
    public class SnapWindow : Window
    {
        private SnapArea current;
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
                    grid.Children.Add(new SnapArea());

                    mainGrid.Children.Add(grid);

                    Grid.SetColumn(grid, i);
                    Grid.SetRow(grid, j);
                }

            Content = mainGrid;
        }

        public Rectangle SelectElementWithPoint(int x, int y)
        {
            if (IsVisible)
            {
                var Point2Window = PointFromScreen(new Point(x, y));

                var element = InputHitTest(Point2Window);
                if (element != null && element is SnapArea)
                {
                    if (current != null)
                    {
                        current.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
                    }

                    var snapArea = current = (SnapArea)element;

                    snapArea.OnHoverStyle();

                    return snapArea.ScreenSnapArea();
                }
                else
                {
                    //TODO imporove here. mooving on different screens, old one preserves the hover style
                    if (current != null)
                    {
                        current.NormalStyle();
                    }
                }
            }

            return Rectangle.Empty;
        }
    }
}