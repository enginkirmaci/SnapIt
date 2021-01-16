using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Controls
{
    public partial class SnapAreaEditor : UserControl
    {
        public SnapControl SnapControl { get; set; }

        public SnapAreaTheme Theme
        {
            get => (SnapAreaTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
         = DependencyProperty.Register("Theme", typeof(SnapAreaTheme), typeof(SnapAreaEditor),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(ThemePropertyChanged)
           });

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var SnapAreaEditor = (SnapAreaEditor)d;
            SnapAreaEditor.Theme = (SnapAreaTheme)e.NewValue;

            if (SnapAreaEditor.Theme != null)
            {
                SnapAreaEditor.Area.Opacity = SnapAreaEditor.Theme.Opacity;
                SnapAreaEditor.Area.Background = SnapAreaEditor.Theme.OverlayBrush;
            }
        }

        public SnapAreaEditor()
        {
            InitializeComponent();

            DesignPanel.Visibility = Visibility.Hidden;
            Area.IsMouseDirectlyOverChanged += SnapAreaEditor_IsMouseDirectlyOverChanged;
        }

        private void SplitVertically_Click(object sender, RoutedEventArgs e)
        {
            Split(SplitDirection.Vertical);
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
            Split(SplitDirection.Horizontal);
        }

        private void Split(SplitDirection direction)
        {
            Point point;
            Size size;

            var rect = this.GetRect();

            if (direction == SplitDirection.Vertical)
            {
                point = new Point((rect.TopLeft.X + rect.BottomRight.X) / 2, rect.TopLeft.Y);
                size = new Size(double.NaN, rect.Height);
            }
            else
            {
                point = new Point(rect.TopLeft.X, (rect.TopLeft.Y + rect.BottomRight.Y) / 2);
                size = new Size(rect.Width, double.NaN);
            }

            var newBorder = new SnapBorder(SnapControl, new SnapAreaTheme());
            newBorder.SetPos(point, size, direction);

            SnapControl.AddBorder(newBorder);
        }

        private void SnapAreaEditor_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsMouseOver)
            {
                DesignPanel.Visibility = Visibility.Visible;
            }
            else
            {
                DesignPanel.Visibility = Visibility.Hidden;
            }
        }

        public Rect GetRect()
        {
            return new Rect(
                new Point(Margin.Left, Margin.Top),
                new Size(ActualWidth, ActualHeight));
        }
    }
}