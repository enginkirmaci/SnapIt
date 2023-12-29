using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using SnapIt.Common.Graphics;
using Point = System.Windows.Point;

namespace SnapIt.Controls;

public class SnapArea : Control
{
    public SnapControl SnapControl { get; set; }

    public bool AreaNumberVisible
    {
        get => (bool)GetValue(AreaNumberVisibleProperty);
        set => SetValue(AreaNumberVisibleProperty, value);
    }

    public static readonly DependencyProperty AreaNumberVisibleProperty
     = DependencyProperty.Register("AreaNumberVisibleProperty", typeof(bool), typeof(SnapArea),
       new FrameworkPropertyMetadata()
       {
           BindsTwoWayByDefault = true,
           PropertyChangedCallback = new PropertyChangedCallback(AreaNumberVisiblePropertyChanged)
       });

    private static void AreaNumberVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var snapArea = (SnapArea)d;
        snapArea.AreaNumberVisible = (bool)e.NewValue;
    }

    public int AreaNumber
    {
        get => (int)GetValue(AreaNumberProperty) + 1;
        set => SetValue(AreaNumberProperty, value);
    }

    public static readonly DependencyProperty AreaNumberProperty
     = DependencyProperty.Register("AreaNumberProperty", typeof(int), typeof(SnapArea),
       new FrameworkPropertyMetadata()
       {
           BindsTwoWayByDefault = true,
           PropertyChangedCallback = new PropertyChangedCallback(AreaNumberPropertyChanged)
       });

    private static void AreaNumberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var snapArea = (SnapArea)d;
        snapArea.AreaNumber = (int)e.NewValue;
    }

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

    public void NormalStyle(bool animate = true)
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
            //if (((SolidColorBrush)area.Background).Color == Theme.OverlayColor)
            //    return;

            area.Background = brush;

            if (animate)
            {
                brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }
            else
            {
                area.Background = Theme.OverlayBrush;
            }
        }
    }

    public void OnHoverStyle(bool animate = true)
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

            if (animate)
            {
                brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            }
            else
            {
                area.Background = Theme.HighlightBrush;
            }
        }
    }

    public Rectangle ScreenSnapArea(Dpi dpi)
    {
        var topLeft = PointToScreen(new Point(SnapControl.AreaPadding, SnapControl.AreaPadding));

        var bottomRight = PointToScreen(new Point(ActualWidth - SnapControl.AreaPadding, ActualHeight - SnapControl.AreaPadding));

        return new Rectangle(
           (int)topLeft.X,
           (int)topLeft.Y,
           (int)bottomRight.X,
           (int)bottomRight.Y,
           dpi);
    }
}