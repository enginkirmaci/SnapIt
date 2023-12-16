using System.Windows.Media;
using SnapIt.Common.Extensions;
using SnapIt.Common.Mvvm;

namespace SnapIt.Common.Entities;

public class SnapAreaTheme : Bindable
{
    private Color highlightColor;
    private Color overlayColor;
    private Color borderColor;
    private int borderThickness;
    private double opacity;
    private SolidColorBrush overlayBrush;
    private SolidColorBrush highlightBrush;
    private SolidColorBrush borderBrush;

    [JsonIgnore]
    public SolidColorBrush OverlayBrush { get => overlayBrush; set => SetProperty(ref overlayBrush, value); }

    [JsonIgnore]
    public SolidColorBrush HighlightBrush { get => highlightBrush; set => SetProperty(ref highlightBrush, value); }

    [JsonIgnore]
    public SolidColorBrush BorderBrush { get => borderBrush; set => SetProperty(ref borderBrush, value); }

    public Color HighlightColor
    {
        get => highlightColor;
        set
        {
            SetProperty(ref highlightColor, value);
            HighlightBrush = new SolidColorBrush(value);

            ThemeChanged?.Invoke();
        }
    }

    public Color OverlayColor
    {
        get => overlayColor;
        set
        {
            SetProperty(ref overlayColor, value);
            OverlayBrush = new SolidColorBrush(value);

            ThemeChanged?.Invoke();
        }
    }

    public Color BorderColor
    {
        get => borderColor;
        set
        {
            SetProperty(ref borderColor, value);
            BorderBrush = new SolidColorBrush(value);

            ThemeChanged?.Invoke();
        }
    }

    public int BorderThickness
    { get => borderThickness; set { SetProperty(ref borderThickness, value); ThemeChanged?.Invoke(); } }

    public double Opacity
    { get => opacity; set { SetProperty(ref opacity, value); ThemeChanged?.Invoke(); } }

    public delegate void ThemeChangedEvent();

    public event ThemeChangedEvent ThemeChanged;

    public SnapAreaTheme()
    {
        HighlightColor = Color.FromArgb(255, 33, 33, 33);
        OverlayColor = Color.FromArgb(255, 99, 99, 99);
        BorderColor = Color.FromArgb(255, 200, 200, 200);
        BorderThickness = 1;
        Opacity = 0.8;
    }

    public SnapAreaTheme(
        SnapIt.Common.Graphics.Color highlightColor,
        SnapIt.Common.Graphics.Color overlayColor,
        SnapIt.Common.Graphics.Color borderColor,
        int borderThickness,
        double opacity
        )
    {
        HighlightColor = highlightColor.Convert();
        OverlayColor = overlayColor.Convert();
        BorderColor = borderColor.Convert();
        Opacity = opacity;
        BorderThickness = borderThickness;
    }

    public SnapAreaTheme Copy()
    {
        return new SnapAreaTheme
        {
            HighlightColor = HighlightColor,
            OverlayColor = OverlayColor,
            BorderColor = BorderColor,
            BorderThickness = BorderThickness,
            Opacity = Opacity
        };
    }
}