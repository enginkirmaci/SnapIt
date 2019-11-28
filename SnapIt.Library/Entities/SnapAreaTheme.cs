using System.Windows.Media;
using Newtonsoft.Json;

namespace SnapIt.Library.Entities
{
    public class SnapAreaTheme : Bindable
    {
        private Color highlightColor;
        private Color overlayColor;
        private SolidColorBrush overlayBrush;
        private SolidColorBrush highlightBrush;

        [JsonIgnore]
        public SolidColorBrush OverlayBrush { get => overlayBrush; set => SetProperty(ref overlayBrush, value); }

        [JsonIgnore]
        public SolidColorBrush HighlightBrush { get => highlightBrush; set => SetProperty(ref highlightBrush, value); }

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

        public delegate void ThemeChangedEvent();

        public event ThemeChangedEvent ThemeChanged;

        public SnapAreaTheme()
        {
            HighlightColor = Color.FromArgb(150, 0, 0, 0);
            OverlayColor = Color.FromArgb(25, 255, 255, 255);
        }

        public SnapAreaTheme Copy()
        {
            return new SnapAreaTheme
            {
                HighlightColor = HighlightColor,
                OverlayColor = OverlayColor,
            };
        }
    }
}