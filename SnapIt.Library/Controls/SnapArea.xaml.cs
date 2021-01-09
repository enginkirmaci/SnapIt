using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Controls
{
    public partial class SnapArea : UserControl
    {
        public SnapControl SnapControl { get; set; }

        public SnapAreaTheme Theme
        {
            get => (SnapAreaTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
         = DependencyProperty.Register("Theme", typeof(SnapAreaTheme), typeof(SnapArea),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(ThemePropertyChanged)
           });

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapArea = (SnapArea)d;
            snapArea.Theme = (SnapAreaTheme)e.NewValue;

            if (snapArea.Theme != null)
            {
                snapArea.Area.Opacity = snapArea.Theme.Opacity;
                snapArea.Area.Background = snapArea.Theme.OverlayBrush;
                snapArea.Border.BorderBrush = snapArea.Theme.BorderBrush;
                snapArea.Border.BorderThickness = new Thickness(snapArea.Theme.BorderThickness);
            }
        }

        public SnapArea()
        {
            InitializeComponent();
        }

        public void NormalStyle()
        {
            Area.Background = Theme.OverlayBrush;
        }

        public void OnHoverStyle()
        {
            Area.Background = Theme.HighlightBrush;
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