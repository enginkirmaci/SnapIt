using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.FindRectangle;

namespace SnapIt.Library.Controls
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
        private SnapBorder snapBorder1;
        private SnapBorder snapBorder2;
        private SnapBorder snapBorder3;
        private SnapBorder snapBorder4;

        public SnapControl()
        {
            InitializeComponent();

            TopBorder = new SnapBorder(new SnapAreaTheme())
            {
                IsDraggable = false
            };

            BottomBorder = new SnapBorder(new SnapAreaTheme())
            {
                IsDraggable = false
            };

            LeftBorder = new SnapBorder(new SnapAreaTheme())
            {
                IsDraggable = false
            };

            RightBorder = new SnapBorder(new SnapAreaTheme())
            {
                IsDraggable = false
            };

            MainGrid.Children.Add(TopBorder);
            MainGrid.Children.Add(BottomBorder);
            MainGrid.Children.Add(LeftBorder);
            MainGrid.Children.Add(RightBorder);

            snapBorder = new SnapBorder(new SnapAreaTheme());
            snapBorder1 = new SnapBorder(new SnapAreaTheme());
            snapBorder2 = new SnapBorder(new SnapAreaTheme());
            snapBorder3 = new SnapBorder(new SnapAreaTheme());
            snapBorder4 = new SnapBorder(new SnapAreaTheme());
            MainGrid.Children.Add(snapBorder);
            MainGrid.Children.Add(snapBorder1);
            MainGrid.Children.Add(snapBorder2);
            MainGrid.Children.Add(snapBorder3);
            MainGrid.Children.Add(snapBorder4);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            TopBorder.SetPos(new Point(0, -SnapBorder.THICKNESSHALF), new Size(MainGrid.ActualWidth, 0), SplitDirection.Horizontal);
            BottomBorder.SetPos(new Point(0, MainGrid.ActualHeight - SnapBorder.THICKNESSHALF), new Size(MainGrid.ActualWidth, 0), SplitDirection.Horizontal);
            LeftBorder.SetPos(new Point(-SnapBorder.THICKNESSHALF, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);
            RightBorder.SetPos(new Point(MainGrid.ActualWidth - SnapBorder.THICKNESSHALF, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);

            snapBorder.SetPos(new Point(150, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);
            snapBorder1.SetPos(new Point(500, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);
            snapBorder2.SetPos(new Point(150, 100), new Size(350, 0), SplitDirection.Horizontal);
            snapBorder3.SetPos(new Point(275, 100), new Size(0, MainGrid.ActualHeight - 100), SplitDirection.Vertical);
            snapBorder4.SetPos(new Point(275, 0), new Size(0, 100), SplitDirection.Vertical);
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

            FindRectangle.Settings settings = new FindRectangle.Settings
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

            var theme = new SnapAreaTheme();
            foreach (var rectangle in rectangles)
            {
                MainAreas.Children.Add(new SnapArea()
                {
                    Margin = new Thickness(rectangle.TopLeft.X, rectangle.TopLeft.Y, 0, 0),
                    Width = rectangle.Width,
                    Height = rectangle.Height,
                    SnapControl = this,
                    Theme = theme
                });
            }
        }
    }
}