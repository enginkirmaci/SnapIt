using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;

namespace SnapIt.Library.Controls
{
    public class SnapArea : Control
    {
        public SnapControl SnapControl { get; set; }

        public Thickness AreaPadding
        {
            get => (Thickness)GetValue(AreaPaddingProperty);
            set => SetValue(AreaPaddingProperty, value);
        }

        public static readonly DependencyProperty AreaPaddingProperty
         = DependencyProperty.Register("AreaPadding", typeof(Thickness), typeof(SnapArea),
           new FrameworkPropertyMetadata()
           {
               DefaultValue = new Thickness(0),
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(AreaPaddingPropertyChanged)
           });

        private static void AreaPaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapAreaEditor = (SnapArea)d;
            snapAreaEditor.AreaPadding = (Thickness)e.NewValue;
        }

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
        }

        public SnapArea()
        {
            Name = $"snaparea_{Guid.NewGuid():N}";
        }

        public void NormalStyle()
        {
            ColorAnimation animation = new ColorAnimation
            {
                From = Theme.HighlightColor,
                To = Theme.OverlayColor,
                Duration = new Duration(TimeSpan.FromMilliseconds(160))
            };

            SolidColorBrush brush = new SolidColorBrush(Theme.OverlayColor);

            var area = this.FindChild<Grid>("Area");
            if (area != null)
            {
                area.Background = brush;
                brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }
        }

        public void OnHoverStyle()
        {
            ColorAnimation animation = new ColorAnimation
            {
                From = Theme.OverlayColor,
                To = Theme.HighlightColor,
                Duration = new Duration(TimeSpan.FromMilliseconds(160))
            };

            SolidColorBrush brush = new SolidColorBrush(Theme.OverlayColor);

            var area = this.FindChild<Grid>("Area");
            if (area != null)
            {
                area.Background = brush;
                brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }
        }

        public Rectangle ScreenSnapArea(Dpi dpi)
        {
            var topLeft = PointToScreen(new Point(SnapControl.AreaPadding, SnapControl.AreaPadding));

            var bottomRight = PointToScreen(new Point(ActualWidth - (SnapControl.AreaPadding), ActualHeight - (SnapControl.AreaPadding)));

            return new Rectangle(
               (int)topLeft.X,
               (int)topLeft.Y,
               (int)bottomRight.X,
               (int)bottomRight.Y,
               dpi);
        }
    }
}