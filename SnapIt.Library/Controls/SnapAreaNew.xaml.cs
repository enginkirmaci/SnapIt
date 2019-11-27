using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Controls
{
    /// <summary>
    /// Interaction logic for SnapAreaNew.xaml
    /// </summary>
    public partial class SnapAreaNew : UserControl
    {
        private static readonly SolidColorBrush transparentBrush = new SolidColorBrush(Colors.Transparent);

        private SnapAreaNew mergedSnapArea;

        public bool HasMergedSnapArea { get; set; }
        public bool IsMergedSnapArea { get; set; }
        public SnapAreaNew ParentSnapArea { get; set; }
        public SplitDirection SplitDirection { get; private set; }

        public bool Transparent
        {
            get => (bool)GetValue(TransparentProperty);
            set => SetValue(TransparentProperty, value);
        }

        public static readonly DependencyProperty TransparentProperty
         = DependencyProperty.Register("Transparent", typeof(bool), typeof(SnapAreaNew),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 PropertyChangedCallback = new PropertyChangedCallback(new PropertyChangedCallback(TransparentPropertyChanged))
             });

        private static void TransparentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((SnapAreaNew)d).Background = ((SnapAreaNew)d).Theme.OverlayBrush;
            }
        }

        public SnapAreaTheme Theme
        {
            get => (SnapAreaTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
         = DependencyProperty.Register("Theme", typeof(SnapAreaTheme), typeof(SnapAreaNew),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(ThemePropertyChanged)
           });

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SnapAreaNew)d).Theme = (SnapAreaTheme)e.NewValue;
        }

        public LayoutArea LayoutArea
        {
            get => (LayoutArea)GetValue(LayoutAreaProperty);
            set => SetValue(LayoutAreaProperty, value);
        }

        public static readonly DependencyProperty LayoutAreaProperty
            = DependencyProperty.Register("LayoutArea", typeof(LayoutArea), typeof(SnapAreaNew),
              new FrameworkPropertyMetadata()
              {
                  BindsTwoWayByDefault = true,
                  PropertyChangedCallback = new PropertyChangedCallback(LayoutAreaPropertyChanged)
              });

        private static void LayoutAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SnapAreaNew)d).ApplyLayout((LayoutArea)e.NewValue, false);
        }

        public SnapAreaNew()
        {
            InitializeComponent();

            Border.Visibility = Visibility.Collapsed;
            DesignPanel.Visibility = Visibility.Collapsed;
            MergedIcon.Visibility = Visibility.Collapsed;
            MergeButton.Visibility = Visibility.Collapsed;
            VerticalSplitter.Visibility = Visibility.Collapsed;
            HorizantalSplitter.Visibility = Visibility.Collapsed;

            Theme = new SnapAreaTheme(); //TODO remove this
        }

        public void GetLayoutAreas(LayoutArea layoutArea)
        {
            foreach (var child in Area.Children)
            {
                if (child is SnapAreaNew)
                {
                    var childSnapArea = child as SnapAreaNew;

                    if (HasMergedSnapArea && mergedSnapArea == childSnapArea)
                    {
                        continue;
                    }

                    var column = Grid.GetColumn(childSnapArea);
                    var row = Grid.GetRow(childSnapArea);

                    var childLayoutArea = new LayoutArea
                    {
                        Column = column,
                        Row = row,
                        Width = Area.ColumnDefinitions.Count > column ? (int?)Area.ColumnDefinitions[column].Width.Value : null,
                        Height = Area.RowDefinitions.Count > row ? (int?)Area.RowDefinitions[row].Height.Value : null
                    };

                    childSnapArea.GetLayoutAreas(childLayoutArea);

                    if (layoutArea.Areas == null)
                    {
                        layoutArea.Areas = new List<LayoutArea>();
                    }

                    layoutArea.Merged = HasMergedSnapArea;
                    layoutArea.Areas.Add(childLayoutArea);
                }
            }
        }

        public void GenerateSnapAreaBoundries(ref List<Rectangle> rectangles, Dpi dpi)
        {
            var hasSnapChild = false;
            foreach (var child in Area.Children)
            {
                if (child is SnapAreaNew)
                {
                    hasSnapChild = true;

                    (child as SnapAreaNew).GenerateSnapAreaBoundries(ref rectangles, dpi);
                }
            }

            if (!hasSnapChild && !IsMergedSnapArea)
            {
                rectangles.Add(ScreenSnapArea(dpi));
            }
        }

        public Rectangle ScreenSnapArea(Dpi dpi)
        {
            var topLeft = PointToScreen(new Point(0, 0));

            var bottomRight = PointToScreen(new Point(ActualWidth, ActualHeight));

            return new Rectangle(
               (int)topLeft.X,
               (int)topLeft.Y,
               (int)bottomRight.X,
               (int)bottomRight.Y,
               dpi);
        }

        public void SetDesignMode(SnapAreaNew parent)
        {
            Border.Visibility = Visibility.Hidden;

            ParentSnapArea = parent;

            if (parent != null)
            {
                RemoveSnapArea.Visibility = Visibility.Visible;
            }
            else
            {
                RemoveSnapArea.Visibility = Visibility.Collapsed;
            }

            DesignPanel.Visibility = Visibility.Visible;

            Border.IsMouseDirectlyOverChanged += SnapArea_IsMouseDirectlyOverChanged;
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
            ParentSnapArea.Area.Children.RemoveRange(0, ParentSnapArea.Area.Children.Count);
            ParentSnapArea.Border.Visibility = Visibility.Visible;

            ParentSnapArea.Area.Children.Add(ParentSnapArea.Border);

            ParentSnapArea.SetDesignMode(ParentSnapArea.ParentSnapArea);

            if (ParentSnapArea.Area.ColumnDefinitions.Count > 0)
            {
                ParentSnapArea.Area.ColumnDefinitions.RemoveRange(0, ParentSnapArea.Area.ColumnDefinitions.Count);
            }
            if (ParentSnapArea.Area.RowDefinitions.Count > 0)
            {
                ParentSnapArea.Area.RowDefinitions.RemoveRange(0, ParentSnapArea.Area.RowDefinitions.Count);
            }
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

        public void ApplyLayout(LayoutArea layoutArea, bool isDesignMode, bool isTransparent = false, SnapAreaNew parent = null)
        {
            Border.Visibility = Visibility.Visible;

            if (parent != null)
            {
                parent.Area.Background = transparentBrush;
            }

            if (layoutArea?.Areas != null && layoutArea.Areas.Count > 0)
            {
                ApplyColumnsAndRows(layoutArea, isDesignMode);

                foreach (var area in layoutArea.Areas)
                {
                    var snapArea = new SnapAreaNew();

                    Area.Children.Add(snapArea);
                    SetColumnRow(snapArea, area.Column, area.Row);
                    snapArea.ApplyLayout(area, isDesignMode, isTransparent, this);
                }

                if (layoutArea.Merged)
                {
                    AddMergedSnapArea(isDesignMode);

                    if (Area.ColumnDefinitions.Count > 0)
                    {
                        Grid.SetColumnSpan(mergedSnapArea, Area.ColumnDefinitions.Count);
                        Grid.SetColumnSpan(MergedIcon, Area.ColumnDefinitions.Count);
                    }

                    if (Area.RowDefinitions.Count > 0)
                    {
                        Grid.SetRowSpan(mergedSnapArea, Area.RowDefinitions.Count);
                        Grid.SetRowSpan(MergedIcon, Area.RowDefinitions.Count);
                    }
                }
            }
            else if (isDesignMode)
            {
                SetDesignMode(parent);
            }
        }

        internal void ApplyColumnsAndRows(LayoutArea layoutArea, bool isDesignMode)
        {
            if (layoutArea.Areas.Count > 0)
            {
                var highestColumn = layoutArea.Areas.Max(area => area.Column);
                var highestRow = layoutArea.Areas.Max(area => area.Row);

                for (int i = 0; i < highestColumn + 1; i++)
                {
                    if (highestColumn != 0)
                    {
                        Area.ColumnDefinitions.Add(new ColumnDefinition());
                    }

                    for (int j = 0; j < highestRow + 1; j++)
                    {
                        if (highestRow != 0)
                        {
                            Area.RowDefinitions.Add(new RowDefinition());
                        }

                        if (isDesignMode)
                        {
                            if (i != 0)
                            {
                                AddGridSplitter(SplitDirection.Vertically, i, j);
                            }

                            if (j != 0)
                            {
                                AddGridSplitter(SplitDirection.Horizantally, i, j);
                            }
                        }
                    }
                }

                foreach (var area in layoutArea.Areas)
                {
                    if (Area.ColumnDefinitions.Count > 0 && area.Width.HasValue)
                    {
                        Area.ColumnDefinitions[area.Column].Width = new GridLength(area.Width.Value, GridUnitType.Star);
                    }

                    if (Area.RowDefinitions.Count > 0 && area.Height.HasValue)
                    {
                        Area.RowDefinitions[area.Row].Height = new GridLength(area.Height.Value, GridUnitType.Star);
                    }
                }
            }
        }

        private void SplitVertically_Click(object sender, RoutedEventArgs e)
        {
            Border.IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;

            Split(this, DesignPanel, SplitDirection.Vertically);
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
            Border.IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;

            Split(this, DesignPanel, SplitDirection.Horizantally);
        }

        private void SetColumnRow(UIElement element, int column, int row)
        {
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
        }

        private void AddMergedSnapArea(bool isDesignMode)
        {
            HasMergedSnapArea = true;

            mergedSnapArea = new SnapAreaNew
            {
                IsMergedSnapArea = true,
                ParentSnapArea = this,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            MergedIcon.Visibility = Visibility.Visible;

            mergedSnapArea.SizeChanged += (s, ev) =>
            {
                var element = s as SnapAreaNew;

                var window = Window.GetWindow(element);

                element.Width = element.ParentSnapArea.ActualWidth * 0.3;
                element.Height = element.ParentSnapArea.ActualHeight * 0.3;

                element.MergedIcon.Width = element.MergedIcon.Height = 64 * (element.Width / 500);
            };

            Area.Children.Add(mergedSnapArea);
        }

        private void Split(SnapAreaNew snapArea, StackPanel designPanel, SplitDirection splitDirection)
        {
            SplitDirection = splitDirection;
            Area.Children.Remove(designPanel);

            var area1 = new SnapAreaNew();
            var area2 = new SnapAreaNew();
            area1.SetDesignMode(snapArea);
            area2.SetDesignMode(snapArea);

            Area.Children.Add(area1);
            Area.Children.Add(area2);

            switch (SplitDirection)
            {
                case SplitDirection.Vertically:

                    Area.ColumnDefinitions.Add(new ColumnDefinition());
                    Area.ColumnDefinitions.Add(new ColumnDefinition());

                    Grid.SetColumn(area2, 1);
                    AddGridSplitter(SplitDirection, 1, 0);

                    break;

                case SplitDirection.Horizantally:

                    Area.RowDefinitions.Add(new RowDefinition());
                    Area.RowDefinitions.Add(new RowDefinition());

                    Grid.SetRow(area2, 1);
                    AddGridSplitter(SplitDirection, 0, 1);

                    break;
            }
        }

        private void AddGridSplitter(SplitDirection direction, int column, int row)
        {
            MergeButton.Visibility = Visibility.Hidden;
            Grid.SetColumn(MergeButton, column);
            Grid.SetRow(MergeButton, row);

            switch (direction)
            {
                case SplitDirection.Vertically:
                    VerticalSplitter.Visibility = Visibility.Visible;
                    Grid.SetColumn(VerticalSplitter, column);
                    Grid.SetRow(VerticalSplitter, row);

                    MergeButton.Margin = new Thickness(-24, 0, 0, 0);
                    MergeButton.HorizontalAlignment = HorizontalAlignment.Left;
                    MergeButton.VerticalAlignment = VerticalAlignment.Center;

                    break;

                case SplitDirection.Horizantally:
                    HorizantalSplitter.Visibility = Visibility.Visible;
                    Grid.SetColumn(HorizantalSplitter, column);
                    Grid.SetRow(HorizantalSplitter, row);

                    MergeButton.Margin = new Thickness(0, -24, 0, 0);
                    MergeButton.HorizontalAlignment = HorizontalAlignment.Center;
                    MergeButton.VerticalAlignment = VerticalAlignment.Top;

                    break;
            }
        }

        public void OnHoverStyle()
        {
            Area.Background = Theme.HighlightBrush;
        }

        public void NormalStyle()
        {
            Area.Background = Theme.OverlayBrush;
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!HasMergedSnapArea)
            {
                AddMergedSnapArea(true);

                if (Area.ColumnDefinitions.Count > 0)
                {
                    Grid.SetColumnSpan(mergedSnapArea, Area.ColumnDefinitions.Count);
                }

                if (Area.RowDefinitions.Count > 0)
                {
                    Grid.SetRowSpan(mergedSnapArea, Area.RowDefinitions.Count);
                }
            }
            else
            {
                Area.Children.Remove(mergedSnapArea);

                HasMergedSnapArea = false;
            }
        }
    }
}