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
        private readonly SnapBorder topBorder;
        private readonly SnapBorder bottomBorder;
        private readonly SnapBorder leftBorder;
        private readonly SnapBorder rightBorder;

        private double overlayMargin = 0;
        private bool firstLoad = true;
        private string currentName;
        private int currentAreaPadding;
        private List<LayoutLine> currentLayoutLines;
        private List<LayoutOverlay> currentLayoutOverlays;

        public int AreaCount { get; set; }

        public bool IsNumberVisible
        {
            get => (bool)GetValue(IsNumberVisibleProperty);
            set => SetValue(IsNumberVisibleProperty, value);
        }

        public static readonly DependencyProperty IsNumberVisibleProperty
         = DependencyProperty.Register("IsNumberVisible", typeof(bool), typeof(SnapControl),
           new FrameworkPropertyMetadata()
           {
               DefaultValue = false,
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(IsNumberVisiblePropertyChanged)
           });

        public void HighlightArea(int number)
        {
            var snapAreas = this.FindChildren<SnapArea>();
            foreach (var snapArea in snapAreas)
            {
                if (snapArea.AreaNumber == number)
                {
                    snapArea.OnHoverStyle();
                }
                else
                {
                    snapArea.NormalStyle();
                }
            }

            var overlays = this.FindChildren<SnapOverlay>();
            foreach (var overlay in overlays)
            {
                if (overlay.AreaNumber == number)
                {
                    overlay.OnHoverStyle();
                }
                else
                {
                    overlay.NormalStyle();
                }
            }
        }

        private static void IsNumberVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapControl = (SnapControl)d;
            snapControl.IsNumberVisible = (bool)e.NewValue;
        }

        public int AreaPadding
        {
            get => (int)GetValue(AreaPaddingProperty);
            set => SetValue(AreaPaddingProperty, value);
        }

        public static readonly DependencyProperty AreaPaddingProperty
         = DependencyProperty.Register("AreaPadding", typeof(int), typeof(SnapControl),
           new FrameworkPropertyMetadata()
           {
               DefaultValue = 0,
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(AreaPaddingPropertyChanged)
           });

        private static void AreaPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapControl = (SnapControl)d;
            snapControl.AreaPadding = (int)e.NewValue;
            //snapControl.Layout.AreaPadding = snapControl.AreaPadding;

            if (snapControl.IsDesignMode)
            {
                var snapAreas = snapControl.FindChildren<SnapAreaEditor>();
                foreach (var snapArea in snapAreas)
                {
                    snapArea.AreaPadding = new Thickness(snapControl.AreaPadding);
                }
            }
            else
            {
                var snapAreas = snapControl.FindChildren<SnapArea>();
                foreach (var snapArea in snapAreas)
                {
                    snapArea.AreaPadding = new Thickness(snapControl.AreaPadding);
                }
            }
        }

        public bool IsOverlayVisible
        {
            get => (bool)GetValue(IsOverlayVisibleProperty);
            set => SetValue(IsOverlayVisibleProperty, value);
        }

        public static readonly DependencyProperty IsOverlayVisibleProperty
         = DependencyProperty.Register("IsOverlayVisible", typeof(bool), typeof(SnapControl),
           new FrameworkPropertyMetadata()
           {
               DefaultValue = true,
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(IsOverlayVisiblePropertyChanged)
           });

        private static void IsOverlayVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapControl = (SnapControl)d;
            snapControl.IsOverlayVisible = (bool)e.NewValue;

            snapControl.MainOverlay.Visibility = snapControl.IsOverlayVisible ? Visibility.Visible : Visibility.Collapsed;
        }

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

        public bool IsPreview
        {
            get => (bool)GetValue(IsPreviewProperty);
            set => SetValue(IsPreviewProperty, value);
        }

        public static readonly DependencyProperty IsPreviewProperty
         = DependencyProperty.Register("IsPreview", typeof(bool), typeof(SnapControl),
           new FrameworkPropertyMetadata()
           {
               DefaultValue = false,
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(IsPreviewPropertyChanged)
           });

        private static void IsPreviewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapControl = (SnapControl)d;
            snapControl.IsPreview = (bool)e.NewValue;
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

            topBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            bottomBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            leftBorder = new SnapBorder(this, Theme) { IsDraggable = false };
            rightBorder = new SnapBorder(this, Theme) { IsDraggable = false };

            SizeChanged += SnapControl_SizeChanged;
            //LayoutUpdated += SnapControl_LayoutUpdated;
        }

        //private void SnapControl_LayoutUpdated(object sender, System.EventArgs e)
        //{
        //    ResetBorderTool();
        //}

        private void SnapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AdoptToScreen();
        }

        public void AddBorder(SnapBorder snapBorder)
        {
            if (IsDesignMode)
            {
                var borderTool = new SnapBorderTool
                {
                    SnapBorder = snapBorder
                };
                MainGrid.Children.Add(borderTool);

                snapBorder.SnapBorderTool = borderTool;
            }

            MainGrid.Children.Add(snapBorder);
            GenerateSnapAreas();
        }

        public void ResetBorderTool()
        {
            var borderTools = MainGrid.FindChildren<SnapBorderTool>();
            foreach (var borderTool in borderTools)
            {
                borderTool.ResetPos();
            }
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
                X = ((ActualWidth - size.Width) / 2) + overlayMargin,
                Y = ((ActualHeight - size.Height) / 2) + overlayMargin
            };

            overlayMargin += 20;

            overlayEditor.SetPos(point, size, null);

            MainOverlay.Children.Add(overlayEditor);

            GenerateSnapOverlays();
        }

        public void RemoveOverlay(SnapOverlayEditor snapOverlayEditor)
        {
            if (snapOverlayEditor.LayoutOverlay != null)
            {
                var layout = Layout.LayoutOverlays.FirstOrDefault(l => l.Point == snapOverlayEditor.LayoutOverlay.Point && l.Size == snapOverlayEditor.LayoutOverlay.Size);
                if (layout != null)
                {
                    Layout.LayoutOverlays.Remove(layout);
                }
            }

            MainOverlay.Children.Remove(snapOverlayEditor);
        }

        public void SetLayoutSize()
        {
            Layout.Size = new Size(ActualWidth, ActualHeight);
        }

        public void Prepare(LayoutStatus status)
        {
            switch (status)
            {
                case LayoutStatus.Saved:
                    Layout.AreaPadding = AreaPadding;
                    Layout.Size = new Size(ActualWidth, ActualHeight);
                    Layout.Status = LayoutStatus.NotSaved;

                    break;

                case LayoutStatus.Ignored:
                    Layout.Name = currentName;
                    Layout.AreaPadding = currentAreaPadding;
                    Layout.LayoutLines = currentLayoutLines;
                    Layout.LayoutOverlays = currentLayoutOverlays;
                    Layout.Status = LayoutStatus.Ignored;

                    break;
            }
        }

        public void ClearLayout()
        {
            Layout.LayoutLines = new List<LayoutLine>();
            Layout.LayoutOverlays = new List<LayoutOverlay>();

            LoadLayout(Layout);
        }

        public void LoadLayout(Layout layout)
        {
            if (firstLoad)
            {
                firstLoad = false;
                currentName = layout.Name;
                currentAreaPadding = layout.AreaPadding;
                currentLayoutLines = new List<LayoutLine>(layout.LayoutLines);
                currentLayoutOverlays = new List<LayoutOverlay>(layout.LayoutOverlays);
            };

            MainGrid.Children.Clear();
            MainFullOverlay.Children.Clear();
            MainOverlay.Children.Clear();

            MainGrid.Children.Add(topBorder);
            MainGrid.Children.Add(bottomBorder);
            MainGrid.Children.Add(leftBorder);
            MainGrid.Children.Add(rightBorder);

            if (layout != null)
            {
                AreaPadding = layout.AreaPadding;

                foreach (var layoutLine in layout.LayoutLines)
                {
                    var snapBorder = new SnapBorder(this, Theme)
                    {
                        LayoutLine = layoutLine
                    };

                    AddBorder(snapBorder);
                }

                if (IsDesignMode)
                {
                    if (layout.LayoutOverlays != null)
                    {
                        foreach (var layoutOverlay in layout.LayoutOverlays)
                        {
                            var overlayEditor = new SnapOverlayEditor(this, Theme)
                            {
                                LayoutOverlay = layoutOverlay,
                                ShowMiniOverlay = true
                            };

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
                            var fullOverlay = new SnapFullOverlay(Theme)
                            {
                                LayoutOverlay = layoutOverlay
                            };

                            var overlay = new SnapOverlay(Theme, fullOverlay)
                            {
                                LayoutOverlay = layoutOverlay,
                            };

                            MainFullOverlay.Children.Add(fullOverlay);
                            MainOverlay.Children.Add(overlay);
                        }
                    }
                }
            }

            AdoptToScreen();
        }

        private void AdoptToScreen()
        {
            AreaCount = 0;

            topBorder.SetPos(new Point(0, -SnapBorder.THICKNESSHALF), new Size(MainGrid.ActualWidth, 0), SplitDirection.Horizontal);
            bottomBorder.SetPos(new Point(0, MainGrid.ActualHeight - SnapBorder.THICKNESSHALF), new Size(MainGrid.ActualWidth, 0), SplitDirection.Horizontal);
            leftBorder.SetPos(new Point(-SnapBorder.THICKNESSHALF, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);
            rightBorder.SetPos(new Point(MainGrid.ActualWidth - SnapBorder.THICKNESSHALF, 0), new Size(0, MainGrid.ActualHeight), SplitDirection.Vertical);

            if (Layout != null && ActualWidth != 0)
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

                    if (IsPreview)
                    {
                        AreaPadding = (int)(Layout.AreaPadding * factorX);
                    }
                }

                if (IsDesignMode)
                {
                    var overlayEditors = this.FindChildren<SnapOverlayEditor>();
                    foreach (var overlayEditor in overlayEditors)
                    {
                        if (overlayEditor.LayoutOverlay != null)
                        {
                            var newPoint = new Point
                            {
                                X = overlayEditor.LayoutOverlay.Point.X * factorX,
                                Y = overlayEditor.LayoutOverlay.Point.Y * factorY
                            };
                            var newSize = new Size
                            {
                                Width = overlayEditor.LayoutOverlay.Size.Width * factorX,
                                Height = overlayEditor.LayoutOverlay.Size.Height * factorY
                            };

                            if (overlayEditor.LayoutOverlay.MiniOverlay != null)
                            {
                                var miniPoint = !IsDesignMode ?
                                    new Point
                                    {
                                        X = (overlayEditor.LayoutOverlay.MiniOverlay.Point.X) * factorX,
                                        Y = (overlayEditor.LayoutOverlay.MiniOverlay.Point.Y) * factorY
                                    } : new Point
                                    {
                                        X = (overlayEditor.LayoutOverlay.MiniOverlay.Point.X - overlayEditor.LayoutOverlay.Point.X) * factorX,
                                        Y = (overlayEditor.LayoutOverlay.MiniOverlay.Point.Y - overlayEditor.LayoutOverlay.Point.Y) * factorY
                                    };

                                var miniSize = new Size
                                {
                                    Width = overlayEditor.LayoutOverlay.MiniOverlay.Size.Width * factorX,
                                    Height = overlayEditor.LayoutOverlay.MiniOverlay.Size.Height * factorY
                                };

                                overlayEditor.SetPos(newPoint, newSize, new LayoutOverlay { Point = miniPoint, Size = miniSize });
                            }
                            else
                            {
                                overlayEditor.SetPos(newPoint, newSize, null);
                            }
                        }
                    }
                }
                else
                {
                    var fullOverlays = this.FindChildren<SnapFullOverlay>();
                    foreach (var fullOverlay in fullOverlays)
                    {
                        if (fullOverlay.LayoutOverlay != null)
                        {
                            var newPoint = new Point
                            {
                                X = fullOverlay.LayoutOverlay.Point.X * factorX,
                                Y = fullOverlay.LayoutOverlay.Point.Y * factorY
                            };
                            var newSize = new Size
                            {
                                Width = fullOverlay.LayoutOverlay.Size.Width * factorX,
                                Height = fullOverlay.LayoutOverlay.Size.Height * factorY
                            };

                            fullOverlay.SetPos(newPoint, newSize);
                        }
                    }

                    var overlays = this.FindChildren<SnapOverlay>();
                    foreach (var overlay in overlays)
                    {
                        overlay.AreaNumber = AreaCount++;
                        overlay.AreaNumberVisible = IsNumberVisible;

                        if (overlay.LayoutOverlay != null)
                        {
                            if (overlay.LayoutOverlay.MiniOverlay != null)
                            {
                                var miniPoint = new Point
                                {
                                    X = overlay.LayoutOverlay.MiniOverlay.Point.X * factorX,
                                    Y = overlay.LayoutOverlay.MiniOverlay.Point.Y * factorY
                                };

                                var miniSize = new Size
                                {
                                    Width = overlay.LayoutOverlay.MiniOverlay.Size.Width * factorX,
                                    Height = overlay.LayoutOverlay.MiniOverlay.Size.Height * factorY
                                };

                                overlay.SetPos(new LayoutOverlay { Point = miniPoint, Size = miniSize });
                            }
                            else
                            {
                                overlay.SetPos(null);
                            }
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
                    var layoutOverlay = overlay.GetOverlay();

                    newLayoutOverlays.Add(layoutOverlay);
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
                        Theme = Theme,
                        AreaPadding = new Thickness(AreaPadding)
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
                        Theme = Theme,
                        AreaNumber = AreaCount++,
                        AreaNumberVisible = IsNumberVisible,
                        AreaPadding = new Thickness(AreaPadding)
                    };

                    MainAreas.Children.Add(snapArea);
                }
            }
        }
    }
}