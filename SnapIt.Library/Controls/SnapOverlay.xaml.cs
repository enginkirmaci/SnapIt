using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Controls
{
    public partial class SnapOverlay : UserControl
    {
        public SnapControl SnapControl { get; }

        public LayoutOverlay LayoutOverlay { get; internal set; }

        public SnapAreaTheme Theme
        {
            get => (SnapAreaTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
         = DependencyProperty.Register("Theme", typeof(SnapAreaTheme), typeof(SnapOverlay),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(ThemePropertyChanged)
           });

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapOverlayEditor = (SnapOverlay)d;
            snapOverlayEditor.Theme = (SnapAreaTheme)e.NewValue;

            if (snapOverlayEditor.Theme != null)
            {
                snapOverlayEditor.Overlay.Opacity = snapOverlayEditor.Theme.Opacity;
                snapOverlayEditor.MiniOverlay.Background = snapOverlayEditor.Theme.OverlayBrush;
                snapOverlayEditor.FullOverlay.Background = snapOverlayEditor.Theme.HighlightBrush;
                snapOverlayEditor.Border.BorderBrush = snapOverlayEditor.Theme.BorderBrush;
                snapOverlayEditor.Border.BorderThickness = new Thickness(snapOverlayEditor.Theme.BorderThickness);
                snapOverlayEditor.FullOverlayBorder.BorderBrush = snapOverlayEditor.Theme.BorderBrush;
                snapOverlayEditor.FullOverlayBorder.BorderThickness = new Thickness(snapOverlayEditor.Theme.BorderThickness);
            }
        }

        public SnapOverlay(SnapControl snapControl, SnapAreaTheme theme)
        {
            InitializeComponent();

            SnapControl = snapControl;
            Theme = theme;

            FullOverlay.Visibility = Visibility.Hidden;

            SizeChanged += SnapOverlay_SizeChanged;
        }

        public void SetPos(Point point, Size size)
        {
            Margin = new Thickness(point.X, point.Y, 0, 0);

            Width = size.Width;
            Height = size.Height;
        }

        public void NormalStyle()
        {
            MiniOverlay.Visibility = Visibility.Visible;
            FullOverlay.Visibility = Visibility.Hidden;
        }

        public void OnHoverStyle()
        {
            MiniOverlay.Visibility = Visibility.Hidden;
            FullOverlay.Visibility = Visibility.Visible;
        }

        private void SnapOverlay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var factor = 0.3;
            MiniOverlay.Width = Width * factor;
            MiniOverlay.Height = Height * factor;

            var iconFactor = 0.2;
            MergedIcon.Width = MergedIcon.Height = MiniOverlay.Height * iconFactor;
        }

        public Rectangle ScreenSnapArea(Dpi dpi)
        {
            var topLeft = PointToScreen(new Point(0, 0));

            var bottomRight = PointToScreen(new Point(ActualWidth, ActualHeight));

            return new Rectangle(
               (int)topLeft.X,
               (int)topLeft.Y,
               (int)bottomRight.X,
               (int)bottomRight.Y,
               dpi);
        }
    }
}