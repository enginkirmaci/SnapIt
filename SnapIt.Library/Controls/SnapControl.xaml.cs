using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.FindRectangle;

namespace SnapIt.Library.Controls
{
    public partial class SnapControl : UserControl
    {
        private double _overlayMargin = 0;
        private SnapBorder TopBorder;
        private SnapBorder BottomBorder;
        private SnapBorder LeftBorder;
        private SnapBorder RightBorder;

        //public SnapAreaTheme Theme { get; set; }
        public SnapAreaTheme Theme
        {
            get => (SnapAreaTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
         = DependencyProperty.Register("Theme", typeof(SnapAreaTheme), typeof(SnapControl),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(ThemePropertyChanged)
           });

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapControl = (SnapControl)d;
            snapControl.Theme = (SnapAreaTheme)e.NewValue;

            if (snapControl.Theme != null)
            {
                if (snapControl.IsDesignMode)
                {
                    var snapAreas = snapControl.FindChildren<SnapAreaEditor>();
                    foreach (var snapArea in snapAreas)
                    {
                        snapArea.Theme = snapControl.Theme;
                    }
                }
                else
                {
                    var snapAreas = snapControl.FindChildren<SnapArea>();
                    foreach (var snapArea in snapAreas)
                    {
                        snapArea.Theme = snapControl.Theme;
                    }
                }
            }
        }

        public bool IsDesignMode
        {
            get => (bool)GetValue(IsDesignModeProperty);
            set => SetValue(IsDesignModeProperty, value);
        }

        public static readonly DependencyProperty IsDesignModeProperty
         = DependencyProperty.Register("IsDesignMode", typeof(bool), typeof(SnapControl),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(IsDesignModePropertyChanged)
           });

        private static void IsDesignModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapControl = (SnapControl)d;

            if ((bool)e.NewValue)
            {
                snapControl.MainGrid.Visibility = Visibility.Visible;
            }
        }

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

            MainGrid.Visibility = Visibility.Collapsed;

            TopBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            BottomBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            LeftBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            RightBorder = new SnapBorder(this, Theme) { IsDraggable = false };

            SizeChanged += SnapControl_SizeChanged;
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

        public void AddOverlay()
        {
            var overlayEditor = new SnapOverlayEditor(this, Theme);

            var scale = 0.4;
            var size = new Size
            {
                Width = ActualWidth * scale,
                Height = ActualHeight * scale
            };
            var point = new Point
            {
                X = ((ActualWidth - size.Width) / 2) + _overlayMargin,
                Y = ((ActualHeight - size.Height) / 2) + _overlayMargin
            };

            _overlayMargin += 20;

            overlayEditor.SetPos(point, size);

            MainOverlay.Children.Add(overlayEditor);

            GenerateSnapOverlays();
        }

        public void RemoveOverlay(SnapOverlayEditor snapOverlayEditor)
        {
            Layout.LayoutOverlays.Remove(snapOverlayEditor.LayoutOverlay);
            MainOverlay.Children.Remove(snapOverlayEditor);
        }

        public void SetLayoutSize()
        {
            Layout.Size = new Size(ActualWidth, ActualHeight);
        }

        public void ClearLayout()
        {
            Layout.LayoutLines = new List<LayoutLine>();

            LoadLayout(Layout);
        }

        public void LoadLayout(Layout layout)
        {
            MainGrid.Children.Clear();
            MainOverlay.Children.Clear();

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

                //snapBorder.SetPos(layoutLine.Point, layoutLine.Size, layoutLine.SplitDirection);

                MainGrid.Children.Add(snapBorder);
            }

            if (IsDesignMode)
            {
                if (layout.LayoutOverlays != null)
                {
                    foreach (var layoutOverlay in layout.LayoutOverlays)
                    {
                        var overlayEditor = new SnapOverlayEditor(this, Theme)
                        {
                            LayoutOverlay = layoutOverlay
                        };

                        //overlayEditor.SetPos(layoutOverlay.Point, layoutOverlay.Size);

                        MainOverlay.Children.Add(overlayEditor);
                    }
                }
            }
            else
            {
                if (layout.LayoutOverlays != null)
                {
                    foreach (var layoutOverlay in layout.LayoutOverlays)
                    {
                        var overlay = new SnapOverlay(this, Theme)
                        {
                            LayoutOverlay = layoutOverlay
                        };

                        //overlayEditor.SetPos(layoutOverlay.Point, layoutOverlay.Size);

                        MainOverlay.Children.Add(overlay);
                    }
                }
            }

            AdoptToScreen();
        }

        private void AdoptToScreen()
        {
            TopBorder.SetPos(new Point(0, -SnapBorder.THICKNESSHALF), new Size(MainGrid.ActualWidth, 0), SplitDirection.Horizontal);
            BottomBorder.SetPos(new Point(0, MainGrid.ActualHeight - SnapBorder.THICKNESSHALF), new Size(MainGrid.ActualWidth, 0), SplitDirection.Horizontal);
            LeftBorder.SetPos(new Point(-SnapBorder.THICKNESSHALF, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);
            RightBorder.SetPos(new Point(MainGrid.ActualWidth - SnapBorder.THICKNESSHALF, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);

            if (Layout != null)
            {
                var factorX = ActualWidth / Layout.Size.Width;
                var factorY = ActualHeight / Layout.Size.Height;

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

                if (IsDesignMode)
                {
                    var overlays = this.FindChildren<SnapOverlayEditor>();
                    foreach (var overlay in overlays)
                    {
                        if (overlay.LayoutOverlay != null)
                        {
                            var newPoint = new Point
                            {
                                X = overlay.LayoutOverlay.Point.X * factorX,
                                Y = overlay.LayoutOverlay.Point.Y * factorY
                            };
                            var newSize = new Size
                            {
                                Width = overlay.LayoutOverlay.Size.Width * factorX,
                                Height = overlay.LayoutOverlay.Size.Height * factorY
                            };

                            overlay.SetPos(newPoint, newSize);
                        }
                    }
                }
                else
                {
                    var overlays = this.FindChildren<SnapOverlay>();
                    foreach (var overlay in overlays)
                    {
                        if (overlay.LayoutOverlay != null)
                        {
                            var newPoint = new Point
                            {
                                X = overlay.LayoutOverlay.Point.X * factorX,
                                Y = overlay.LayoutOverlay.Point.Y * factorY
                            };
                            var newSize = new Size
                            {
                                Width = overlay.LayoutOverlay.Size.Width * factorX,
                                Height = overlay.LayoutOverlay.Size.Height * factorY
                            };

                            overlay.SetPos(newPoint, newSize);
                        }
                    }
                }
            }

            GenerateSnapAreas();
        }

        public void GenerateSnapOverlays()
        {
            if (IsDesignMode)
            {
                var overlays = this.FindChildren<SnapOverlayEditor>();

                var newLayoutOverlays = new List<LayoutOverlay>();

                foreach (var overlay in overlays)
                {
                    var line = overlay.GetOverlay();

                    newLayoutOverlays.Add(line);
                }

                if (Layout != null)
                {
                    Layout.LayoutOverlays = newLayoutOverlays;
                }
            }
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

            var newLayoutLines = new List<LayoutLine>();

            foreach (var border in borders.Where(b => b.IsDraggable))
            {
                var line = border.GetLine();

                newLayoutLines.Add(line);

                settings.Segments.Add(new Segment
                {
                    Location = new System.Drawing.Point((int)line.Start.X, (int)line.Start.Y),
                    EndLocation = new System.Drawing.Point((int)line.End.X, (int)line.End.Y),
                    Orientation = line.SplitDirection
                });
            }

            if (IsDesignMode && Layout != null)
            {
                Layout.LayoutLines = newLayoutLines;
            }

            settings.Calculate();

            var rectangles = settings.GetRectangles();

            foreach (var rectangle in rectangles)
            {
                if (IsDesignMode)
                {
                    var snapArea = new SnapAreaEditor()
                    {
                        Margin = new Thickness(rectangle.TopLeft.X, rectangle.TopLeft.Y, 0, 0),
                        Width = rectangle.Width,
                        Height = rectangle.Height,
                        SnapControl = this,
                        Theme = Theme
                    };

                    MainAreas.Children.Add(snapArea);
                }
                else
                {
                    var snapArea = new SnapArea()
                    {
                        Margin = new Thickness(rectangle.TopLeft.X, rectangle.TopLeft.Y, 0, 0),
                        Width = rectangle.Width,
                        Height = rectangle.Height,
                        SnapControl = this,
                        Theme = Theme
                    };

                    MainAreas.Children.Add(snapArea);
                }
            }
        }
    }
}