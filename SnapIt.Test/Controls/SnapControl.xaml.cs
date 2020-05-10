using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SnapIt.Library.Entities;
using SnapIt.Test.Library;

namespace SnapIt.Test.Controls
{
    /// <summary>
    /// Interaction logic for SnapControl.xaml
    /// </summary>
    public partial class SnapControl : UserControl
    {
        private SnapBorder TopBorder;
        private SnapBorder BottomBorder;
        private SnapBorder LeftBorder;
        private SnapBorder RightBorder;

        public SnapControl()
        {
            InitializeComponent();

            TopBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Horizontally,
                IsDraggable = false,
                Width = MainGrid.ActualWidth,
                Margin = new Thickness(0, 0, 0, 0)
            };

            BottomBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Horizontally,
                IsDraggable = false,
                Width = MainGrid.ActualWidth,
                Margin = new Thickness(0, MainGrid.ActualHeight, 0, 0)
            };

            LeftBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Vertically,
                IsDraggable = false,
                Height = MainGrid.ActualHeight,
                Margin = new Thickness(0, 0, 0, 0)
            };

            RightBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Vertically,
                IsDraggable = false,
                Height = MainGrid.ActualHeight,
                Margin = new Thickness(MainGrid.ActualWidth, 0, 0, 0)
            };

            MainGrid.Children.Add(TopBorder);
            MainGrid.Children.Add(BottomBorder);
            MainGrid.Children.Add(LeftBorder);
            MainGrid.Children.Add(RightBorder);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            TopBorder.Width = MainGrid.ActualWidth;
            BottomBorder.Width = MainGrid.ActualWidth;
            BottomBorder.Margin = new Thickness(0, MainGrid.ActualHeight, 0, 0);
            LeftBorder.Height = MainGrid.ActualHeight;
            RightBorder.Height = MainGrid.ActualHeight;
            RightBorder.Margin = new Thickness(MainGrid.ActualWidth, 0, 0, 0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var lines = new List<Line>{
                TopBorder.GetLine(),
                BottomBorder.GetLine(),
                LeftBorder.GetLine(),
                RightBorder.GetLine()
            };

            var rectangles = SweepLine.GetRectangles(lines);

            foreach (var rectangle in rectangles)
            {
                MainGrid.Children.Add(new SnapArea()
                {
                    Margin = new Thickness(rectangle.TopRight.X, rectangle.TopRight.Y, 0, 0),
                    Width = rectangle.BottomLeft.X - rectangle.TopRight.X,
                    Height = rectangle.BottomLeft.Y - rectangle.TopRight.Y
                });
            }
        }
    }
}