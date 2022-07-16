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
            typeof(bool), typeof(SnapOverlayEditor), new PropertyMetadata(ShowMiniOverlayPropertyChanged));

        private static void ShowMiniOverlayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapOverlayEditor = (SnapOverlayEditor)d;
            snapOverlayEditor.ShowMiniOverlay = (bool)e.NewValue;

            if (snapOverlayEditor.ShowMiniOverlay)
            {
                snapOverlayEditor.RemoveButton.Visibility = Visibility.Collapsed;
                snapOverlayEditor.FullOverlay.Visibility = Visibility.Hidden;
                snapOverlayEditor.MiniOverlay.Visibility = Visibility.Visible;
            }
            else
            {
                snapOverlayEditor.RemoveButton.Visibility = Visibility.Visible;
                snapOverlayEditor.FullOverlay.Visibility = Visibility.Visible;
                snapOverlayEditor.MiniOverlay.Visibility = Visibility.Hidden;
            }
        }

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

        public string PositionX
        {
            get => (string)GetValue(PositionXProperty);
            set => SetValue(PositionXProperty, value);
        }

        public static readonly DependencyProperty PositionXProperty = DependencyProperty.Register(nameof(PositionX),
            typeof(string), typeof(SnapOverlayEditor), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string PositionY
        {
            get => (string)GetValue(PositionYProperty);
            set => SetValue(PositionYProperty, value);
        }

        public static readonly DependencyProperty PositionYProperty = DependencyProperty.Register(nameof(PositionY),
            typeof(string), typeof(SnapOverlayEditor), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string PositionWidth
        {
            get => (string)GetValue(PositionWidthProperty);
            set => SetValue(PositionWidthProperty, value);
        }

        public static readonly DependencyProperty PositionWidthProperty = DependencyProperty.Register(nameof(PositionWidth),
            typeof(string), typeof(SnapOverlayEditor), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string PositionHeight
        {
            get => (string)GetValue(PositionHeightProperty);
            set => SetValue(PositionHeightProperty, value);
        }

        public static readonly DependencyProperty PositionHeightProperty = DependencyProperty.Register(nameof(PositionHeight),
            typeof(string), typeof(SnapOverlayEditor), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public SnapOverlayEditor(SnapControl snapControl, SnapAreaTheme theme)
        {
            InitializeComponent();

            SnapControl = snapControl;
            Theme = theme;

            MiniOverlay.SizeChanged += MiniOverlay_SizeChanged;

            ShowMiniOverlay = false;
            RemoveButton.Visibility = Visibility.Visible;
            FullOverlay.Visibility = Visibility.Visible;
            MiniOverlay.Visibility = Visibility.Hidden;

            DesignPanel.Visibility = Visibility.Hidden;
            OutlineBorder.Visibility = Visibility.Hidden;
            PositionGrid.Visibility = Visibility.Hidden;
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

        public void ResetDesignPanelButtons()
        {
            if (ShowMiniOverlay)
            {
                DesignPanel.Margin = new Thickness(
                    MiniOverlay.Margin.Left + MiniOverlay.Width - DesignPanel.ActualWidth - 20,
                    MiniOverlay.Margin.Top + 20,
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

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            ResetDesignPanelButtons();

            PositionX = Margin.Left.ToString("0.00");
            PositionY = Margin.Top.ToString("0.00");
            PositionWidth = Width.ToString("0.00");
            PositionHeight = Height.ToString("0.00");
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            DesignPanel.Visibility = Visibility.Hidden;
            OutlineBorder.Visibility = Visibility.Hidden;
            PositionGrid.Visibility = Visibility.Hidden;
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
                        PositionGrid.Visibility = Visibility.Visible;
                        selectedElement = this;
                    }
                }

                _mouseHitType = SetHitType(selectedElement, Mouse.GetPosition(selectedElement));
                SetMouseCursor();

                ResetDesignPanelButtons();
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
                        if (offset_x == 0 && offset_y == 0)
                        {
                            return;
                        }

                        SetPos(point, size);

                        PositionX = Margin.Left.ToString("0.00");
                        PositionY = Margin.Top.ToString("0.00");
                        PositionWidth = Width.ToString("0.00");
                        PositionHeight = Height.ToString("0.00");
                    }

                    ResetDesignPanelButtons();

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

                MiniOverlay.Visibility = Visibility.Hidden;
                FullOverlay.Visibility = Visibility.Visible;
                RemoveButton.Visibility = Visibility.Visible;
            }
            else
            {
                ShowMiniOverlay = true;

                RemoveButton.Visibility = Visibility.Collapsed;
                FullOverlay.Visibility = Visibility.Hidden;
                MiniOverlay.Visibility = Visibility.Visible;
            }
        }

        private void SetPosButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(PositionX, out var positionX)
                && double.TryParse(PositionY, out var positionY)
                && double.TryParse(PositionWidth, out var positionWidth)
                && double.TryParse(PositionHeight, out var positionHeight))
            {
                SetPos(new Point(positionX, positionY), new Size(positionWidth, positionHeight));
                SnapControl.GenerateSnapOverlays();
            }
        }
    }
}