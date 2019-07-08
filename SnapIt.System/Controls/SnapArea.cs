using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using SnapIt.Entities;
using Point = System.Windows.Point;

namespace SnapIt.Controls
{
    public class SnapArea : Border
    {
        private readonly SolidColorBrush backgroundBrush = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
        private readonly SolidColorBrush borderBrush = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
        private readonly SolidColorBrush backgroundOnHoverBrush = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));
        private readonly SolidColorBrush splitterBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
        private readonly SolidColorBrush buttonBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));

        private readonly PackIcon splitVerticallyIcon = new PackIcon { Kind = PackIconKind.ArrowSplitVertical };
        private readonly PackIcon splitHorizontallyIcon = new PackIcon { Kind = PackIconKind.ArrowSplitHorizontal };
        private readonly PackIcon removeSnapAreaIcon = new PackIcon { Kind = PackIconKind.Remove };

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
            var topLeft = PointToScreen(new Point(0, 0));

            var bottomRight = PointToScreen(new Point(ActualWidth, ActualHeight));

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
            Background = new SolidColorBrush(Colors.Transparent);

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
                Margin = new Thickness(2),
                Padding = new Thickness(12, 2, 12, 2),
                Background = buttonBackgroundBrush,
                Style = Application.Current.TryFindResource("MaterialDesignFlatButton") as Style,
            };
            splitVertically.Content = splitVerticallyIcon;
            splitVertically.Click += SplitVertically_Click;
            designPanel.Children.Add(splitVertically);

            var splitHorizantally = new Button
            {
                Margin = new Thickness(2),
                Padding = new Thickness(12, 2, 12, 2),
                Background = buttonBackgroundBrush,
                Style = Application.Current.TryFindResource("MaterialDesignFlatButton") as Style
            };
            splitHorizantally.Content = splitHorizontallyIcon;
            splitHorizantally.Click += SplitHorizantally_Click;
            designPanel.Children.Add(splitHorizantally);

            if (parent != null)
            {
                var removeSnapArea = new Button
                {
                    Margin = new Thickness(2),
                    Padding = new Thickness(12, 2, 12, 2),
                    Background = buttonBackgroundBrush,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Style = Application.Current.TryFindResource("MaterialDesignFlatButton") as Style
                };
                removeSnapArea.Content = removeSnapAreaIcon;
                removeSnapArea.Click += RemoveSnapArea_Click;

                designPanel.Children.Add(removeSnapArea);
            }

            MainArea.Children.Add(designPanel);

            IsMouseDirectlyOverChanged += SnapArea_IsMouseDirectlyOverChanged;
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
            ParentSnapArea.Child = ParentSnapArea.MainArea = new Grid();

            ParentSnapArea.SetDesignMode(ParentSnapArea.ParentSnapArea);
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