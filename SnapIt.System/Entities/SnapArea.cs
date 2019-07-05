using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SnapIt.Entities
{
    public class SnapArea : Border
    {
        private readonly SolidColorBrush backgroundBrush = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
        private readonly SolidColorBrush borderBrush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
        private readonly SolidColorBrush backgroundOnHoverBrush = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));

        public Grid SnapAreaGrid { get; set; }

        public SnapArea()
        {
            Child = SnapAreaGrid = new Grid();

            Background = backgroundBrush;
            BorderBrush = borderBrush;
            BorderThickness = new Thickness(1, 1, 1, 1);
        }

        public Rectangle ScreenSnapArea()
        {
            var topLeft = PointToScreen(new System.Windows.Point(0, 0));

            var bottomRight = PointToScreen(new System.Windows.Point(ActualWidth, ActualHeight));

            return new Rectangle(
               (int)topLeft.X,
               (int)topLeft.Y,
               (int)bottomRight.X,
               (int)bottomRight.Y);
        }

        public void OnHoverStyle()
        {
            Background = backgroundOnHoverBrush;
        }

        public void NormalStyle()
        {
            Background = backgroundBrush;
        }
    }
}