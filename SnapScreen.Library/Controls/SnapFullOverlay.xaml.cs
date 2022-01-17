using System.Windows;
using System.Windows.Controls;
using SnapScreen.Library.Entities;

namespace SnapScreen.Library.Controls
{
    public partial class SnapFullOverlay : UserControl
    {
        public LayoutOverlay LayoutOverlay { get; internal set; }

        public SnapAreaTheme Theme
        {
            get => (SnapAreaTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
         = DependencyProperty.Register("Theme", typeof(SnapAreaTheme), typeof(SnapFullOverlay),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(ThemePropertyChanged)
           });

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapOverlayEditor = (SnapFullOverlay)d;
            snapOverlayEditor.Theme = (SnapAreaTheme)e.NewValue;

            if (snapOverlayEditor.Theme != null)
            {
                snapOverlayEditor.Overlay.Opacity = snapOverlayEditor.Theme.Opacity;
                snapOverlayEditor.Overlay.Background = snapOverlayEditor.Theme.HighlightBrush;
                snapOverlayEditor.OverlayBorder.BorderBrush = snapOverlayEditor.Theme.BorderBrush;
                snapOverlayEditor.OverlayBorder.BorderThickness = new Thickness(snapOverlayEditor.Theme.BorderThickness);
            }
        }

        public SnapFullOverlay(SnapAreaTheme theme)
        {
            InitializeComponent();

            Theme = theme;

            Overlay.Visibility = Visibility.Hidden;
        }

        public void SetPos(Point point, Size size)
        {
            Margin = new Thickness(point.X, point.Y, 0, 0);

            Width = size.Width;
            Height = size.Height;
        }

        public void NormalStyle()
        {
            Overlay.Visibility = Visibility.Hidden;
        }

        public void OnHoverStyle()
        {
            Overlay.Visibility = Visibility.Visible;
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