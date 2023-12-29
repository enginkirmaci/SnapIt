using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using SnapIt.Common.Graphics;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace SnapIt.Controls;

public partial class SnapOverlay : UserControl
{
    public LayoutOverlay LayoutOverlay { get; internal set; }

    public SnapFullOverlay SnapFullOverlay { get; }

    public bool AreaNumberVisible
    {
        get => (bool)GetValue(AreaNumberVisibleProperty);
        set => SetValue(AreaNumberVisibleProperty, value);
    }

    public static readonly DependencyProperty AreaNumberVisibleProperty
     = DependencyProperty.Register("AreaNumberVisibleProperty", typeof(bool), typeof(SnapOverlay),
       new FrameworkPropertyMetadata()
       {
           BindsTwoWayByDefault = true,
           PropertyChangedCallback = new PropertyChangedCallback(AreaNumberVisiblePropertyChanged)
       });

    private static void AreaNumberVisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var snapOverlay = (SnapOverlay)d;
        snapOverlay.AreaNumberVisible = (bool)e.NewValue;
    }

    public int AreaNumber
    {
        get => (int)GetValue(AreaNumberProperty) + 1;
        set => SetValue(AreaNumberProperty, value);
    }

    public static readonly DependencyProperty AreaNumberProperty
     = DependencyProperty.Register("AreaNumberProperty", typeof(int), typeof(SnapOverlay),
       new FrameworkPropertyMetadata()
       {
           BindsTwoWayByDefault = true,
           PropertyChangedCallback = new PropertyChangedCallback(AreaNumberPropertyChanged)
       });

    private static void AreaNumberPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var snapOverlay = (SnapOverlay)d;
        snapOverlay.AreaNumber = (int)e.NewValue;
    }

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
        DataContext = this;

        Name = $"snapoverlay_{Guid.NewGuid():N}";

        Theme = theme;
        SnapFullOverlay = snapFullOverlay;

        SizeChanged += SnapOverlay_SizeChanged;
    }

    public void SetPos(FrameworkElement element, Point point, Size size)
    {
        element.Margin = new Thickness(point.X, point.Y, 0, 0);

        element.Width = size.Width;
        element.Height = size.Height;
    }

    public void SetPos(LayoutOverlay layoutOverlay)
    {
        if (layoutOverlay != null)
        {
            SetPos(this, layoutOverlay.Point.Convert(), layoutOverlay.Size.Convert());
        }
        else
        {
            var factor = 0.3;
            Width = SnapFullOverlay.Width * factor;
            Height = SnapFullOverlay.Height * factor;

            Margin = new Thickness(
               SnapFullOverlay.Margin.Left + (SnapFullOverlay.Width / 2 - Width / 2),
               SnapFullOverlay.Margin.Top + (SnapFullOverlay.Height / 2 - Height / 2),
                0, 0);
        }
    }

    public void NormalStyle()
    {
        Overlay.Background = Theme.OverlayBrush;
        Border.Visibility = Visibility.Visible;
        MergedIcon.Visibility = Visibility.Visible;

        SnapFullOverlay.NormalStyle();
    }

    public void OnHoverStyle(bool animate = true)
    {
        Overlay.Background = Brushes.Transparent;
        Border.Visibility = Visibility.Hidden;
        MergedIcon.Visibility = Visibility.Hidden;

        SnapFullOverlay.OnHoverStyle(animate);
    }

    private void SnapOverlay_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var iconFactor = 0.2;
        if (ActualWidth > ActualHeight)
        {
            var size = MergedIcon.Width = MergedIcon.Height = ActualWidth * iconFactor;
            MergedIcon.FontSize = size > 0 ? size : 1;
        }
        else
        {
            var size = MergedIcon.Width = MergedIcon.Height = ActualHeight * iconFactor;
            MergedIcon.FontSize = size > 0 ? size : 1;
        }
    }

    public Rectangle ScreenSnapArea(Dpi dpi)
    {
        return SnapFullOverlay.ScreenSnapArea(dpi);
    }
}