using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.FindRectangle;
using SnapIt.Library.Services;

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

        public SnapAreaTheme Theme { get; set; }
        //public List<LayoutLine> SnapLines { get; set; }
        //public List<LayoutArea> LayoutAreas { get; set; }

        public Layout Layout
        {
            get => (Layout)GetValue(LayoutProperty);
            set => SetValue(LayoutProperty, value);
        }

        public static readonly DependencyProperty LayoutProperty
         = DependencyProperty.Register("Layout", typeof(Layout), typeof(SnapControl),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(LayoutPropertyChanged)
           });

        private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapControl = (SnapControl)d;
            var layout = (Layout)e.NewValue;

            snapControl.LoadLayout(layout);
        }

        public SnapControl()
        {
            InitializeComponent();

            Theme = new SnapAreaTheme();

            TopBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            BottomBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            LeftBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            RightBorder = new SnapBorder(this, Theme) { IsDraggable = false };

            this.SizeChanged += SnapControl_SizeChanged;
        }

        private void SnapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdoptToScreen();
        }

        public void AddBorder(SnapBorder snapBorder)
        {
            MainGrid.Children.Add(snapBorder);
            GenerateSnapAreas();
        }

        public void GenerateSnapAreas()
        {
            MainAreas.Children.Clear();

            var borders = this.FindChildren<SnapBorder>();

            FindRectangle.Settings settings = new FindRectangle.Settings
            {
                Size = new System.Drawing.Size((int)ActualWidth, (int)ActualHeight),
                Segments = new List<Segment>()
            };

            Layout.LayoutLines = new List<LayoutLine>();
            foreach (var border in borders.Where(b => b.IsDraggable))
            {
                var line = border.GetLine();

                Layout.LayoutLines.Add(line);

                settings.Segments.Add(new Segment
                {
                    Location = new System.Drawing.Point((int)line.Start.X, (int)line.Start.Y),
                    EndLocation = new System.Drawing.Point((int)line.End.X, (int)line.End.Y),
                    Orientation = border.SplitDirection
                });
            }

            settings.Calculate();

            var rectangles = settings.GetRectangles();

            Layout.LayoutAreas = new List<LayoutArea>();

            foreach (var rectangle in rectangles)
            {
                Layout.LayoutAreas.Add(rectangle.GetLayoutArea());

                MainAreas.Children.Add(new SnapArea()
                {
                    Margin = new Thickness(rectangle.TopLeft.X, rectangle.TopLeft.Y, 0, 0),
                    Width = rectangle.Width,
                    Height = rectangle.Height,
                    SnapControl = this,
                    Theme = Theme
                });
            }
        }

        public void SaveLayout()
        {
            Layout.Guid = Guid.NewGuid();
            Layout.IsSaved = false;
            Layout.Name = "New layout";
            Layout.Size = new Size(ActualWidth, ActualHeight);

            FileOperationService fileOperationService = new FileOperationService();
            fileOperationService.SaveLayout(Layout);
        }

        public void LoadLayout(Layout layout)
        {
            MainGrid.Children.Clear();
            //MainAreas.Children.Clear();

            MainGrid.Children.Add(TopBorder);
            MainGrid.Children.Add(BottomBorder);
            MainGrid.Children.Add(LeftBorder);
            MainGrid.Children.Add(RightBorder);

            foreach (var layoutLine in layout.LayoutLines)
            {
                var snapBorder = new SnapBorder(this, Theme)
                {
                    LayoutLine = layoutLine
                };
                MainGrid.Children.Add(snapBorder);

                snapBorder.SetPos(layoutLine.Point, layoutLine.Size, layoutLine.SplitDirection);
            }

            //foreach (var layoutArea in layout.LayoutAreas)
            //{
            //    MainAreas.Children.Add(new SnapArea()
            //    {
            //        Margin = layoutArea.Margin,
            //        Width = layoutArea.Width,
            //        Height = layoutArea.Height,
            //        SnapControl = this,
            //        Theme = Theme
            //    });
            //}

            AdoptToScreen();
        }

        private void AdoptToScreen()
        {
            TopBorder.SetPos(new Point(0, -SnapBorder.THICKNESSHALF), new Size(MainGrid.ActualWidth, 0), SplitDirection.Horizontal);
            BottomBorder.SetPos(new Point(0, MainGrid.ActualHeight - SnapBorder.THICKNESSHALF), new Size(MainGrid.ActualWidth, 0), SplitDirection.Horizontal);
            LeftBorder.SetPos(new Point(-SnapBorder.THICKNESSHALF, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);
            RightBorder.SetPos(new Point(MainGrid.ActualWidth - SnapBorder.THICKNESSHALF, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);

            var factorX = ActualWidth / Layout.Size.Width;
            var factorY = ActualHeight / Layout.Size.Height;

            FindRectangle.Settings settings = new FindRectangle.Settings
            {
                Size = new System.Drawing.Size((int)ActualWidth, (int)ActualHeight),
                Segments = new List<Segment>()
            };

            var borders = this.FindChildren<SnapBorder>();
            foreach (var border in borders.Where(b => b.IsDraggable))
            {
                if (border.LayoutLine != null)
                {
                    var newPoint = new Point
                    {
                        X = border.LayoutLine.Point.X * factorX,
                        Y = border.LayoutLine.Point.Y * factorY
                    };
                    var newSize = new Size
                    {
                        Width = border.LayoutLine.Size.Width * factorX,
                        Height = border.LayoutLine.Size.Height * factorY
                    };

                    border.SetPos(newPoint, newSize, border.LayoutLine.SplitDirection);
                }
            }

            GenerateSnapAreas();
        }
    }
}