using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Controls
{
    /// <summary>
    /// Interaction logic for SnapArea.xaml
    /// </summary>
    public partial class SnapArea : UserControl
    {
        public SnapControl SnapControl { get; set; }

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

            if (snapArea.Theme != null)
            {
                snapArea.Area.Opacity = snapArea.Theme.Opacity;
                snapArea.Area.Background = snapArea.Theme.OverlayBrush;
                //snapArea.Border.BorderBrush = snapArea.Theme.BorderBrush;
                //snapArea.Border.BorderThickness = new Thickness(snapArea.Theme.BorderThickness);
                //snapArea.Border.Visibility = Visibility.Visible;
            }
        }

        public SnapArea()
        {
            InitializeComponent();

            DesignPanel.Visibility = Visibility.Hidden;
            Area.IsMouseDirectlyOverChanged += SnapArea_IsMouseDirectlyOverChanged;
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

            var newBorder = new SnapBorder(new SnapAreaTheme());
            newBorder.SetPos(point, size, direction);

            SnapControl.AddBorder(newBorder);
        }

        private void SnapArea_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        //private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        //{
        //}

        public Rect GetRect()
        {
            return new Rect(
                new Point(Margin.Left, Margin.Top),
                new Size(ActualWidth, ActualHeight));
        }
    }
}