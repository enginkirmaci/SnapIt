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

        public SnapControl SnapControl { get; }

        public LayoutOverlay LayoutOverlay { get; internal set; }

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

            DesignPanel.Visibility = Visibility.Hidden;
            FullOverlay.Visibility = Visibility.Hidden;
            FullOverlay.PreviewKeyDown += FullOverlay_PreviewKeyDown;
            FullOverlay.Focusable = true;

            SizeChanged += SnapOverlayEditor_SizeChanged;
        }

        private void SnapOverlayEditor_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var factor = 0.3;
            MiniOverlay.Width = Width * factor;
            MiniOverlay.Height = Height * factor;

            var iconFactor = 0.2;
            MergedIcon.Width = MergedIcon.Height = MiniOverlay.Height * iconFactor;
        }

        public LayoutOverlay GetOverlay()
        {
            return new LayoutOverlay
            {
                Point = new Point(Margin.Left, Margin.Top),
                Size = new Size(Width, Height)
            };
        }

        public void SetPos(Point point, Size size)
        {
            Margin = new Thickness(point.X, point.Y, 0, 0);

            Width = size.Width;
            Height = size.Height;
        }

        private bool isEscapeKeyPressed = false;

        private void FullOverlay_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            isEscapeKeyPressed = e.Key == Key.Escape;
            Cursor = Cursors.Arrow;
            MiniOverlay.Visibility = Visibility.Visible;
            FullOverlay.Visibility = Visibility.Hidden;
            DesignPanel.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (isEscapeKeyPressed) { return; }

            _lastPointInContiner = Mouse.GetPosition(SnapControl);

            _mouseHitType = SetHitType(Mouse.GetPosition(this));
            SetMouseCursor();

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

            isEscapeKeyPressed = false;

            MiniOverlay.Visibility = Visibility.Visible;
            FullOverlay.Visibility = Visibility.Hidden;
            DesignPanel.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Keyboard.Focus(FullOverlay);
            if (isEscapeKeyPressed) { return; }

            MiniOverlay.Visibility = Visibility.Hidden;
            FullOverlay.Visibility = Visibility.Visible;
            DesignPanel.Visibility = Visibility.Visible;

            if (!IsMouseCaptured)
            {
                _mouseHitType = SetHitType(Mouse.GetPosition(this));
                SetMouseCursor();
            }
            else
            {
                var mousePosition = Mouse.GetPosition(SnapControl);
                double offset_x = mousePosition.X - _lastPointInContiner.X;
                double offset_y = mousePosition.Y - _lastPointInContiner.Y;

                double new_x = this.Margin.Left;
                double new_y = this.Margin.Top;
                double new_width = this.Width;
                double new_height = this.Height;

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

                    SetPos(point, size);

                    _lastPointInContiner = Mouse.GetPosition(SnapControl);
                }
            }
        }

        private ResizeHitType SetHitType(Point point)
        {
            double left = 0;
            double top = 0;
            double right = left + this.Width;
            double bottom = top + this.Height;
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
    }
}