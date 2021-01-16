using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Controls
{
    public partial class SnapOverlay : UserControl
    {
        public LayoutOverlay LayoutOverlay { get; internal set; }

        public SnapFullOverlay SnapFullOverlay { get; }

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
                snapOverlayEditor.Overlay.Background = snapOverlayEditor.Theme.OverlayBrush;
                snapOverlayEditor.Border.BorderBrush = snapOverlayEditor.Theme.BorderBrush;
                snapOverlayEditor.Border.BorderThickness = new Thickness(snapOverlayEditor.Theme.BorderThickness);
            }
        }

        public SnapOverlay(SnapAreaTheme theme, SnapFullOverlay snapFullOverlay)
        {
            InitializeComponent();

            Theme = theme;
            SnapFullOverlay = snapFullOverlay;

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
            Overlay.Background = Theme.OverlayBrush;
            Border.Visibility = Visibility.Visible;
            MergedIcon.Visibility = Visibility.Visible;

            SnapFullOverlay.NormalStyle();
        }

        public void OnHoverStyle()
        {
            Overlay.Background = Brushes.Transparent;
            Border.Visibility = Visibility.Hidden;
            MergedIcon.Visibility = Visibility.Hidden;

            SnapFullOverlay.OnHoverStyle();
        }

        private void SnapOverlay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var factor = 0.3;
            Overlay.Width = Width * factor;
            Overlay.Height = Height * factor;

            var iconFactor = 0.2;
            MergedIcon.Width = MergedIcon.Height = Overlay.Height * iconFactor;
        }

        public Rectangle ScreenSnapArea(Dpi dpi)
        {
            return SnapFullOverlay.ScreenSnapArea(dpi);
        }
    }
}