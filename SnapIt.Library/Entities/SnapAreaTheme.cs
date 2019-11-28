using System.Windows.Media;
using Newtonsoft.Json;

namespace SnapIt.Library.Entities
{
    public class SnapAreaTheme : Bindable
    {
        private Color highlightColor;
        private Color overlayColor;
        private Color borderColor;
        private int borderThickness;
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

        public int BorderThickness { get => borderThickness; set { SetProperty(ref borderThickness, value); ThemeChanged?.Invoke(); } }

        public delegate void ThemeChangedEvent();

        public event ThemeChangedEvent ThemeChanged;

        public SnapAreaTheme()
        {
            HighlightColor = Color.FromArgb(150, 0, 0, 0);
            OverlayColor = Color.FromArgb(25, 255, 255, 255);
            BorderColor = Color.FromArgb(255, 200, 200, 200);
            borderThickness = 1;
        }

        public SnapAreaTheme Copy()
        {
            return new SnapAreaTheme
            {
                HighlightColor = HighlightColor,
                OverlayColor = OverlayColor,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness
            };
        }
    }
}