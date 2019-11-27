using System.Windows.Media;
using Newtonsoft.Json;

namespace SnapIt.Library.Entities
{
    public class SnapAreaTheme
    {
        private Color highlightColor;
        private Color overlayColor;

        [JsonIgnore]
        public SolidColorBrush OverlayBrush { get; private set; }

        [JsonIgnore]
        public SolidColorBrush HighlightBrush { get; private set; }

        public Color HighlightColor
        {
            get => highlightColor;
            set
            {
                highlightColor = value;
                HighlightBrush = new SolidColorBrush(value);
            }
        }

        public Color OverlayColor
        {
            get => overlayColor;
            set
            {
                overlayColor = value;
                OverlayBrush = new SolidColorBrush(value);
            }
        }

        public SnapAreaTheme()
        {
            HighlightColor = Color.FromArgb(150, 0, 0, 0);
            OverlayColor = Color.FromArgb(25, 255, 255, 255);
        }
    }
}