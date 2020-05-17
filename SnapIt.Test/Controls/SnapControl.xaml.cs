using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SliceRectangleSample;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;

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
        private SnapBorder snapBorder;

        public SnapControl()
        {
            InitializeComponent();

            TopBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Horizontal,
                IsDraggable = false,
                Width = MainGrid.ActualWidth,
                Margin = new Thickness(0, 0, 0, 0)
            };

            BottomBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Horizontal,
                IsDraggable = false,
                Width = MainGrid.ActualWidth,
                Margin = new Thickness(0, MainGrid.ActualHeight, 0, 0)
            };

            LeftBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Vertical,
                IsDraggable = false,
                Height = MainGrid.ActualHeight,
                Margin = new Thickness(0, 0, 0, 0)
            };

            RightBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Vertical,
                IsDraggable = false,
                Height = MainGrid.ActualHeight,
                Margin = new Thickness(MainGrid.ActualWidth, 0, 0, 0)
            };

            MainGrid.Children.Add(TopBorder);
            MainGrid.Children.Add(BottomBorder);
            MainGrid.Children.Add(LeftBorder);
            MainGrid.Children.Add(RightBorder);

            snapBorder = new SnapBorder()
            {
                SplitDirection = SplitDirection.Vertical,
                Height = MainGrid.ActualHeight,
                Margin = new Thickness(MainGrid.ActualWidth * 0.4, 0, 0, 0)
            };

            MainGrid.Children.Add(snapBorder);
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

            snapBorder.Height = MainGrid.ActualHeight;
            snapBorder.Margin = new Thickness(MainGrid.ActualWidth * 0.4, 0, 0, 0);
        }

        public void AddBorder(SnapBorder snapBorder)
        {
            MainGrid.Children.Add(snapBorder);
            GenerateSnapAreas();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GenerateSnapAreas();
        }

        private void GenerateSnapAreas()
        {
            MainAreas.Children.Clear();

            var borders = this.FindChildren<SnapBorder>();

            Settings settings = new Settings
            {
                Size = new System.Drawing.Size((int)ActualWidth, (int)ActualHeight),
                Segments = new List<Segment>()
            };

            foreach (var border in borders)
            {
                var line = border.GetLine();

                settings.Segments.Add(new Segment
                {
                    Location = new System.Drawing.Point((int)line.Start.X, (int)line.Start.Y),
                    EndLocation = new System.Drawing.Point((int)line.End.X, (int)line.End.Y),
                    Orientation = border.SplitDirection
                });
            }

            settings.Calculate();

            var rectangles = settings.GetRectangles();

            foreach (var rectangle in rectangles)
            {
                MainAreas.Children.Add(new SnapArea()
                {
                    Margin = new Thickness(rectangle.TopLeft.X, rectangle.TopLeft.Y, 0, 0),
                    Width = rectangle.Width,
                    Height = rectangle.Height,
                    SnapControl = this
                });
            }
        }
    }
}