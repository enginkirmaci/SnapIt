using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Controls
{
    public partial class SnapOverlayEditor : UserControl
    {
        private const double GAP = 12;

        private Point _lastPointInContiner;
        private ResizeHitType _mouseHitType = ResizeHitType.None;
        private FrameworkElement selectedElement = null;

        public SnapControl SnapControl { get; }

        public LayoutOverlay LayoutOverlay { get; internal set; }

        public bool ShowMiniOverlay
        {
            get => (bool)GetValue(ShowMiniOverlayProperty);
            set => SetValue(ShowMiniOverlayProperty, value);
        }

        public static readonly DependencyProperty ShowMiniOverlayProperty = DependencyProperty.Register(nameof(ShowMiniOverlay),
            typeof(bool), typeof(SnapOverlayEditor), new PropertyMetadata(null));

        public SnapAreaTheme Theme
        {
            get => (SnapAreaTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
         = DependencyProperty.Register("Theme", typeof(SnapAreaTheme), typeof(SnapOverlayEditor),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(ThemePropertyChanged)
           });

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapOverlayEditor = (SnapOverlayEditor)d;
            snapOverlayEditor.Theme = (SnapAreaTheme)e.NewValue;

            if (snapOverlayEditor.Theme != null)
            {
                snapOverlayEditor.Overlay.Opacity = snapOverlayEditor.Theme.Opacity;
                snapOverlayEditor.MiniOverlay.Background = snapOverlayEditor.Theme.OverlayBrush;
                snapOverlayEditor.FullOverlay.Background = snapOverlayEditor.Theme.OverlayBrush;
                snapOverlayEditor.Border.BorderBrush = snapOverlayEditor.Theme.BorderBrush;
                snapOverlayEditor.Border.BorderThickness = new Thickness(snapOverlayEditor.Theme.BorderThickness);
                snapOverlayEditor.FullOverlayBorder.BorderBrush = snapOverlayEditor.Theme.BorderBrush;
                snapOverlayEditor.FullOverlayBorder.BorderThickness = new Thickness(5);
            }
        }

        public SnapOverlayEditor(SnapControl snapControl, SnapAreaTheme theme)
        {
            InitializeComponent();

            SnapControl = snapControl;
            Theme = theme;

            MiniOverlay.SizeChanged += MiniOverlay_SizeChanged;

            ShowMiniOverlay = false;
            FullOverlay.Visibility = Visibility.Visible;
            MiniOverlay.Visibility = Visibility.Hidden;

            Loaded += SnapOverlayEditor_Loaded;

            DesignPanel.Visibility = Visibility.Hidden;
            OutlineBorder.Visibility = Visibility.Hidden;
        }

        private void SnapOverlayEditor_Loaded(object sender, RoutedEventArgs e)
        {
            ResetDesignPanel();
        }

        private void MiniOverlay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var iconFactor = 0.2;
            if (MiniOverlay.Width > MiniOverlay.Height)
            {
                MergedIcon.FontSize = MergedIcon.Width = MergedIcon.Height = MiniOverlay.Width * iconFactor;
            }
            else
            {
                MergedIcon.FontSize = MergedIcon.Width = MergedIcon.Height = MiniOverlay.Height * iconFactor;
            }
        }

        public LayoutOverlay GetOverlay()
        {
            return new LayoutOverlay
            {
                Point = new Point(Margin.Left, Margin.Top),
                Size = new Size(Width, Height),
                MiniOverlay = new LayoutOverlay
                {
                    Point = new Point(Margin.Left + MiniOverlay.Margin.Left, Margin.Top + MiniOverlay.Margin.Top),
                    Size = new Size(MiniOverlay.Width, MiniOverlay.Height)
                }
            };
        }

        public void ResetDesignPanel()
        {
            if (ShowMiniOverlay)
            {
                DesignPanel.Margin = new Thickness(
                    MiniOverlay.Margin.Left + (MiniOverlay.Width / 2) - (DesignPanel.ActualWidth / 2),
                    MiniOverlay.Margin.Top + (MiniOverlay.Height / 2) - (DesignPanel.ActualHeight / 2),
                    0, 0);
            }
            else
            {
                DesignPanel.Margin = new Thickness(
                    (Width / 2) - (DesignPanel.ActualWidth / 2),
                    (Height / 2) - (DesignPanel.ActualHeight / 2),
                    0, 0);
            }
        }

        public void SetPos(FrameworkElement element, Point point, Size size)
        {
            element.Margin = new Thickness(point.X, point.Y, 0, 0);

            element.Width = size.Width;
            element.Height = size.Height;
        }

        public void SetPos(Point point, Size size)
        {
            SetPos(this, point, size);
        }

        public void SetPos(Point point, Size size, LayoutOverlay miniOverlay)
        {
            SetPos(this, point, size);

            if (miniOverlay != null)
            {
                SetPos(MiniOverlay, miniOverlay.Point, miniOverlay.Size);
            }
            else
            {
                var factor = 0.3;
                MiniOverlay.Width = Width * factor;
                MiniOverlay.Height = Height * factor;

                MiniOverlay.Margin = new Thickness(
                    (Width / 2) - MiniOverlay.Width / 2,
                    (Height / 2) - MiniOverlay.Height / 2,
                    0, 0);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _mouseHitType = SetHitType(selectedElement, Mouse.GetPosition(selectedElement));
            SetMouseCursor();

            _lastPointInContiner = Mouse.GetPosition(SnapControl);

            if (_mouseHitType == ResizeHitType.None) { return; }

            CaptureMouse();

            Border.Background = Theme.HighlightBrush;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            ReleaseMouseCapture();

            Border.Background = Theme.OverlayBrush;

            SnapControl.GenerateSnapOverlays();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            DesignPanel.Visibility = Visibility.Hidden;
            OutlineBorder.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            DesignPanel.Visibility = Visibility.Visible;
            OutlineBorder.Visibility = Visibility.Visible;

            Keyboard.Focus(FullOverlay);

            if (!IsMouseCaptured)
            {
                var element = InputHitTest(Mouse.GetPosition(this));

                if (element != null)
                {
                    if (ShowMiniOverlay)
                    {
                        selectedElement = MiniOverlay;
                    }
                    else
                    {
                        selectedElement = this;
                    }
                }

                _mouseHitType = SetHitType(selectedElement, Mouse.GetPosition(selectedElement));
                SetMouseCursor();
            }
            else
            {
                var mousePosition = Mouse.GetPosition(SnapControl);
                double offset_x = mousePosition.X - _lastPointInContiner.X;
                double offset_y = mousePosition.Y - _lastPointInContiner.Y;

                double new_x = selectedElement.Margin.Left;
                double new_y = selectedElement.Margin.Top;
                double new_width = selectedElement.Width;
                double new_height = selectedElement.Height;

                switch (_mouseHitType)
                {
                    case ResizeHitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        break;

                    case ResizeHitType.UL:
                        new_x += offset_x;
                        new_y += offset_y;
                        new_width -= offset_x;
                        new_height -= offset_y;
                        break;

                    case ResizeHitType.UR:
                        new_y += offset_y;
                        new_width += offset_x;
                        new_height -= offset_y;
                        break;

                    case ResizeHitType.LR:
                        new_width += offset_x;
                        new_height += offset_y;
                        break;

                    case ResizeHitType.LL:
                        new_x += offset_x;
                        new_width -= offset_x;
                        new_height += offset_y;
                        break;

                    case ResizeHitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        break;

                    case ResizeHitType.R:
                        new_width += offset_x;
                        break;

                    case ResizeHitType.B:
                        new_height += offset_y;
                        break;

                    case ResizeHitType.T:
                        new_y += offset_y;
                        new_height -= offset_y;
                        break;
                }

                if ((new_width > 0) && (new_height > 0))
                {
                    var point = new Point(new_x, new_y);
                    var size = new Size(new_width, new_height);

                    DevMode.Log($"{selectedElement.Name}, {selectedElement.GetType()}");

                    if (selectedElement.Name == "MiniOverlay")
                    {
                        SetPos(selectedElement, point, size);
                    }
                    else
                    {
                        SetPos(point, size);
                    }

                    ResetDesignPanel();

                    _lastPointInContiner = Mouse.GetPosition(SnapControl);
                }
            }
        }

        private ResizeHitType SetHitType(FrameworkElement element, Point point)
        {
            double left = 0;
            double top = 0;
            double right = left + element.Width;
            double bottom = top + element.Height;
            if (point.X < left) return ResizeHitType.None;
            if (point.X > right) return ResizeHitType.None;
            if (point.Y < top) return ResizeHitType.None;
            if (point.Y > bottom) return ResizeHitType.None;

            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return ResizeHitType.UL;
                if (bottom - point.Y < GAP) return ResizeHitType.LL;
                return ResizeHitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return ResizeHitType.UR;
                if (bottom - point.Y < GAP) return ResizeHitType.LR;
                return ResizeHitType.R;
            }
            if (point.Y - top < GAP) return ResizeHitType.T;
            if (bottom - point.Y < GAP) return ResizeHitType.B;
            return ResizeHitType.Body;
        }

        private void SetMouseCursor()
        {
            Cursor desired_cursor = Cursors.Arrow;
            switch (_mouseHitType)
            {
                case ResizeHitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;

                case ResizeHitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;

                case ResizeHitType.UL:
                case ResizeHitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;

                case ResizeHitType.LL:
                case ResizeHitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;

                case ResizeHitType.T:
                case ResizeHitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;

                case ResizeHitType.L:
                case ResizeHitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            if (Cursor != desired_cursor)
            {
                Cursor = desired_cursor;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            SnapControl.RemoveOverlay(this);
        }

        private void ToggleOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (ShowMiniOverlay)
            {
                ShowMiniOverlay = false;

                FullOverlay.Visibility = Visibility.Visible;
                MiniOverlay.Visibility = Visibility.Hidden;
            }
            else
            {
                ShowMiniOverlay = true;

                FullOverlay.Visibility = Visibility.Hidden;
                MiniOverlay.Visibility = Visibility.Visible;
            }

            ResetDesignPanel();
        }
    }
}