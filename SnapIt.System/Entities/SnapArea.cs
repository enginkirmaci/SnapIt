using System;
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
        private readonly SolidColorBrush splitterBackgroundBrush = new SolidColorBrush(Color.FromArgb(100, 150, 150, 150));

        private StackPanel designPanel;

        public Grid MainArea { get; set; }
        public SnapArea ParentSnapArea { get; set; }

        public SnapArea()
        {
            Child = MainArea = new Grid();
            Name = "SnapArea_" + new Random().Next().ToString();

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

        public void SetDesignMode(SnapArea parent)
        {
            ParentSnapArea = parent;

            designPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
                Visibility = Visibility.Hidden
            };

            var splitVertically = new Button
            {
                Margin = new Thickness(5),
                Content = "|"
            };
            splitVertically.Click += SplitVertically_Click;
            designPanel.Children.Add(splitVertically);

            var splitHorizantally = new Button
            {
                Margin = new Thickness(5),
                Content = "-"
            };
            splitHorizantally.Click += SplitHorizantally_Click;
            designPanel.Children.Add(splitHorizantally);

            if (parent != null)
            {
                var removeSnapArea = new Button
                {
                    Margin = new Thickness(20, 5, 5, 5),
                    Content = "X",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                };
                removeSnapArea.Click += RemoveSnapArea_Click;

                designPanel.Children.Add(removeSnapArea);
            }

            MainArea.Children.Add(designPanel);

            IsMouseDirectlyOverChanged += SnapArea_IsMouseDirectlyOverChanged;
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
            //Parent.Child = Parent.MainArea = new Grid();

            //Parent.SetDesignMode(Parent.Parent);

            ParentSnapArea?.RemoveSnapArea(this);
        }

        public void RemoveSnapArea(SnapArea snapArea)
        {
            //Parent.MainArea.Children.Remove(snapArea);

            SnapArea otherChild = null;
            foreach (var child in ParentSnapArea.MainArea.Children)
            {
                if (child is SnapArea && child != snapArea)
                {
                    otherChild = child as SnapArea;
                    break;
                }
            }
            ParentSnapArea.RemoveLogicalChild(otherChild);

            ParentSnapArea.MainArea.Children.Remove(otherChild);

            ParentSnapArea.Child = otherChild.Child;

            otherChild.ParentSnapArea = ParentSnapArea.ParentSnapArea;
            //ParentSnapArea = otherChild;
            //ParentSnapArea.InvalidateVisual();
        }

        private void SnapArea_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (designPanel != null)
            {
                if (IsMouseOver)
                {
                    designPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    designPanel.Visibility = Visibility.Hidden;
                }
            }
        }

        private void SplitVertically_Click(object sender, RoutedEventArgs e)
        {
            IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;
            MainArea.Children.Remove(designPanel);
            designPanel = null;

            MainArea.ColumnDefinitions.Add(new ColumnDefinition());
            MainArea.ColumnDefinitions.Add(new ColumnDefinition());

            var leftArea = new SnapArea();
            var rightArea = new SnapArea();
            leftArea.SetDesignMode(this);
            rightArea.SetDesignMode(this);

            MainArea.Children.Add(leftArea);
            MainArea.Children.Add(rightArea);

            Grid.SetColumn(rightArea, 1);

            var splitter = new GridSplitter
            {
                Width = 10,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                ResizeDirection = GridResizeDirection.Auto,
                Background = splitterBackgroundBrush
            };

            MainArea.Children.Add(splitter);

            Grid.SetColumn(splitter, 1);
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
            IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;
            MainArea.Children.Remove(designPanel);
            designPanel = null;

            MainArea.RowDefinitions.Add(new RowDefinition());
            MainArea.RowDefinitions.Add(new RowDefinition());

            var topArea = new SnapArea();
            var bottomArea = new SnapArea();
            topArea.SetDesignMode(this);
            bottomArea.SetDesignMode(this);

            MainArea.Children.Add(topArea);
            MainArea.Children.Add(bottomArea);

            Grid.SetRow(bottomArea, 1);

            var splitter = new GridSplitter
            {
                Height = 10,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                ResizeDirection = GridResizeDirection.Auto,
                Background = splitterBackgroundBrush
            };

            MainArea.Children.Add(splitter);

            Grid.SetRow(splitter, 1);
        }
    }
}