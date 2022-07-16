using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;

namespace SnapIt.Library.Controls
{
    public class SnapFullOverlay : Control
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
        }

        public SnapFullOverlay(SnapAreaTheme theme)
        {
            Theme = theme;
        }

        public void SetPos(Point point, Size size)
        {
            Margin = new Thickness(point.X, point.Y, 0, 0);

            Width = size.Width;
            Height = size.Height;
        }

        public void NormalStyle()
        {
            Visibility = Visibility.Hidden;
        }

        public void OnHoverStyle()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = Theme.Opacity,
                Duration = new Duration(TimeSpan.FromMilliseconds(160))
            };

            var overlay = this.FindChild<Grid>("Overlay");
            if (overlay != null)
            {
                Visibility = Visibility.Visible;

                overlay.BeginAnimation(OpacityProperty, animation);
            }
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