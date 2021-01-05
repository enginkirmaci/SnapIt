using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SnapIt.Library.Controls
{
    public partial class SnapAreaOld : UserControl
    {
        private static readonly SolidColorBrush transparentBrush = new SolidColorBrush(Colors.Transparent);

        private SnapAreaOld mergedSnapArea;

        public bool HasMergedSnapArea { get; set; }
        public bool IsMergedSnapArea { get; set; }
        public SnapAreaOld ParentSnapArea { get; set; }
        public SplitDirection SplitDirection { get; private set; }

        public bool IsDesignMode
        {
            get => (bool)GetValue(IsDesignModeProperty);
            set => SetValue(IsDesignModeProperty, value);
        }

        public static readonly DependencyProperty IsDesignModeProperty
         = DependencyProperty.Register("IsDesignMode", typeof(bool), typeof(SnapAreaOld),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 PropertyChangedCallback = new PropertyChangedCallback(new PropertyChangedCallback(IsDesignModePropertyChanged))
             });

        private static void IsDesignModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapArea = (SnapAreaOld)d;
            snapArea.IsDesignMode = (bool)e.NewValue;
        }

        public bool IsMergeIconHidden
        {
            get => (bool)GetValue(IsMergeIconHiddenProperty);
            set => SetValue(IsMergeIconHiddenProperty, value);
        }

        public static readonly DependencyProperty IsMergeIconHiddenProperty
         = DependencyProperty.Register("IsMergeIconHidden", typeof(bool), typeof(SnapAreaOld),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 PropertyChangedCallback = new PropertyChangedCallback(new PropertyChangedCallback(IsMergeIconHiddenPropertyChanged))
             });

        private static void IsMergeIconHiddenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapArea = (SnapAreaOld)d;
            snapArea.IsMergeIconHidden = (bool)e.NewValue;

            if (snapArea.IsMergeIconHidden)
            {
                snapArea.MergedIcon.Visibility = Visibility.Hidden;

                var areas = snapArea.Area.FindChildren<SnapAreaOld>();

                foreach (var area in areas)
                {
                    area.MergedIcon.Visibility = Visibility.Hidden;
                }
            }
        }

        public SnapAreaTheme Theme
        {
            get => (SnapAreaTheme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public static readonly DependencyProperty ThemeProperty
         = DependencyProperty.Register("Theme", typeof(SnapAreaTheme), typeof(SnapAreaOld),
           new FrameworkPropertyMetadata()
           {
               BindsTwoWayByDefault = true,
               PropertyChangedCallback = new PropertyChangedCallback(ThemePropertyChanged)
           });

        private static void ThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapArea = (SnapAreaOld)d;
            snapArea.Theme = (SnapAreaTheme)e.NewValue;

            if (snapArea.Theme != null)
            {
                snapArea.Area.Opacity = snapArea.Theme.Opacity;
                snapArea.Area.Background = snapArea.Theme.OverlayBrush;
                snapArea.MergedIcon.Foreground = snapArea.Theme.BorderBrush;
                snapArea.Border.BorderBrush = snapArea.Theme.BorderBrush;
                snapArea.Border.BorderThickness = new Thickness(snapArea.Theme.BorderThickness);
                snapArea.Border.Visibility = Visibility.Visible;

                if (snapArea.LayoutArea?.Areas != null && snapArea.LayoutArea.Areas.Count > 0)
                {
                    snapArea.Area.Background = transparentBrush;
                    snapArea.Border.Visibility = Visibility.Hidden;
                }
            }

            var areas = snapArea.Area.FindChildren<SnapAreaOld>();

            foreach (var area in areas)
            {
                area.Theme = snapArea.Theme;

                area.Area.Opacity = snapArea.Theme.Opacity;
                area.Area.Background = area.Theme.OverlayBrush;
                area.MergedIcon.Foreground = snapArea.Theme.BorderBrush;
                area.Border.BorderBrush = snapArea.Theme.BorderBrush;
                area.Border.BorderThickness = new Thickness(snapArea.Theme.BorderThickness);
                area.Border.Visibility = Visibility.Visible;

                if (area.LayoutArea?.Areas != null && area.LayoutArea.Areas.Count > 0)
                {
                    area.Area.Background = transparentBrush;
                    area.Border.Visibility = Visibility.Hidden;
                }
            }
        }

        public LayoutArea LayoutArea
        {
            get => (LayoutArea)GetValue(LayoutAreaProperty);
            set => SetValue(LayoutAreaProperty, value);
        }

        public static readonly DependencyProperty LayoutAreaProperty
            = DependencyProperty.Register("LayoutArea", typeof(LayoutArea), typeof(SnapAreaOld),
              new FrameworkPropertyMetadata()
              {
                  BindsTwoWayByDefault = true,
                  PropertyChangedCallback = new PropertyChangedCallback(LayoutAreaPropertyChanged)
              });

        private static void LayoutAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapArea = (SnapAreaOld)d;
            snapArea.LayoutArea = (LayoutArea)e.NewValue;

            snapArea.ApplyLayout(snapArea.ParentSnapArea);
        }

        public SnapAreaOld()
        {
            InitializeComponent();

            MergedIcon.Visibility = Visibility.Hidden;
            DesignPanel.Visibility = Visibility.Hidden;
            MergeButton.Visibility = Visibility.Hidden;
            VerticalSplitter.Visibility = Visibility.Hidden;
            HorizantalSplitter.Visibility = Visibility.Hidden;
        }

        public void SetPreview()
        {
            var firstSnapArea = this.FindChildren<SnapAreaOld>().FirstOrDefault();

            if (firstSnapArea != null)
            {
                firstSnapArea.Area.Background = Theme.HighlightBrush;
            }
        }

        public void GetLayoutAreas(LayoutArea layoutArea)
        {
            foreach (var child in Area.Children)
            {
                if (child is SnapAreaOld)
                {
                    var childSnapArea = child as SnapAreaOld;

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

                    //layoutArea.Merged = HasMergedSnapArea;
                    layoutArea.Areas.Add(childLayoutArea);
                }
            }
        }

        public void GenerateSnapAreaBoundries(ref List<Rectangle> rectangles, Dpi dpi)
        {
            var hasSnapChild = false;
            foreach (var child in Area.Children)
            {
                if (child is SnapAreaOld)
                {
                    hasSnapChild = true;

                    (child as SnapAreaOld).GenerateSnapAreaBoundries(ref rectangles, dpi);
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

        private void ApplyLayout(SnapAreaOld parent = null)
        {
            if (LayoutArea?.Areas != null && LayoutArea.Areas.Count > 0)
            {
                ApplyColumnsAndRows(LayoutArea);

                foreach (var area in LayoutArea.Areas)
                {
                    var snapArea = new SnapAreaOld
                    {
                        Theme = Theme,
                        IsMergeIconHidden = IsMergeIconHidden,
                        IsDesignMode = IsDesignMode,
                        ParentSnapArea = this,
                        LayoutArea = area
                    };

                    Area.Children.Add(snapArea);
                    SetColumnRow(snapArea, area.Column, area.Row);
                }

                //if (LayoutArea.Merged)
                {
                    AddMergedSnapArea();

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
            else if (IsDesignMode)
            {
                SetDesignMode(parent);
            }
        }

        internal void ApplyColumnsAndRows(LayoutArea layoutArea)
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

                        if (IsDesignMode)
                        {
                            if (i != 0)
                            {
                                AddGridSplitter(SplitDirection.Vertical, i, j);
                            }

                            if (j != 0)
                            {
                                AddGridSplitter(SplitDirection.Horizontal, i, j);
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

        private void AddMergedSnapArea()
        {
            HasMergedSnapArea = true;

            mergedSnapArea = new SnapAreaOld
            {
                Theme = Theme,
                IsMergedSnapArea = true,
                ParentSnapArea = this,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            if (IsDesignMode)
            {
                Panel.SetZIndex(mergedSnapArea, -1);
            }

            if (!IsMergeIconHidden)
            {
                mergedSnapArea.MergedIcon.Visibility = Visibility.Visible;
            }

            mergedSnapArea.SizeChanged += (s, ev) =>
            {
                var element = s as SnapAreaOld;

                element.Width = element.ParentSnapArea.ActualWidth * 0.3;
                element.Height = element.ParentSnapArea.ActualHeight * 0.3;

                var calculated = 64 * (element.Width / 500);

                if (calculated < 32)
                {
                    calculated = 32;
                }

                element.MergedIcon.Width = element.MergedIcon.Height = calculated;
            };

            Area.Children.Add(mergedSnapArea);
        }

        public void OnHoverStyle()
        {
            Area.Background = Theme.HighlightBrush;
        }

        public void NormalStyle()
        {
            Area.Background = Theme.OverlayBrush;
        }

        private void SetColumnRow(UIElement element, int column, int row)
        {
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
        }

        #region Design Mode

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

        public void SetDesignMode(SnapAreaOld parent)
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

            Area.IsMouseDirectlyOverChanged += SnapArea_IsMouseDirectlyOverChanged;
        }

        private void SplitVertically_Click(object sender, RoutedEventArgs e)
        {
            Area.IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;

            Split(this, DesignPanel, SplitDirection.Vertical);
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
            Area.IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;

            Split(this, DesignPanel, SplitDirection.Horizontal);
        }

        private void Split(SnapAreaOld snapArea, Grid designPanel, SplitDirection splitDirection)
        {
            SplitDirection = splitDirection;
            designPanel.Visibility = Visibility.Hidden;

            var area1 = new SnapAreaOld
            {
                Theme = Theme,
                IsMergeIconHidden = IsMergeIconHidden,
                IsDesignMode = IsDesignMode
            };
            var area2 = new SnapAreaOld
            {
                Theme = Theme,
                IsMergeIconHidden = IsMergeIconHidden,
                IsDesignMode = IsDesignMode
            };
            area1.ApplyLayout(snapArea);
            area2.ApplyLayout(snapArea);

            Area.Children.Add(area1);
            Area.Children.Add(area2);

            switch (SplitDirection)
            {
                case SplitDirection.Vertical:

                    Area.ColumnDefinitions.Add(new ColumnDefinition());
                    Area.ColumnDefinitions.Add(new ColumnDefinition());

                    Grid.SetColumn(area2, 1);
                    AddGridSplitter(SplitDirection, 1, 0);

                    break;

                case SplitDirection.Horizontal:

                    Area.RowDefinitions.Add(new RowDefinition());
                    Area.RowDefinitions.Add(new RowDefinition());

                    Grid.SetRow(area2, 1);
                    AddGridSplitter(SplitDirection, 0, 1);

                    break;
            }
        }

        private void AddGridSplitter(SplitDirection direction, int column, int row)
        {
            MergeButton.Visibility = Visibility.Visible;
            Grid.SetColumn(MergeButton, column);
            Grid.SetRow(MergeButton, row);

            switch (direction)
            {
                case SplitDirection.Vertical:
                    VerticalSplitter.Visibility = Visibility.Visible;
                    Grid.SetColumn(VerticalSplitter, column);
                    Grid.SetRow(VerticalSplitter, row);

                    MergeButton.Margin = new Thickness(-24, 0, 0, 0);
                    MergeButton.HorizontalAlignment = HorizontalAlignment.Left;
                    MergeButton.VerticalAlignment = VerticalAlignment.Center;

                    break;

                case SplitDirection.Horizontal:
                    HorizantalSplitter.Visibility = Visibility.Visible;
                    Grid.SetColumn(HorizantalSplitter, column);
                    Grid.SetRow(HorizantalSplitter, row);

                    MergeButton.Margin = new Thickness(0, -24, 0, 0);
                    MergeButton.HorizontalAlignment = HorizontalAlignment.Center;
                    MergeButton.VerticalAlignment = VerticalAlignment.Top;

                    break;
            }
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
            var childSnapAreas = ParentSnapArea.Area.FindChildren<SnapAreaOld>();
            foreach (var child in childSnapAreas.ToList())
            {
                ParentSnapArea.Area.Children.Remove(child);
            }

            ParentSnapArea.Border.Visibility = Visibility.Visible;
            ParentSnapArea.MergeButton.Visibility = Visibility.Hidden;

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

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!HasMergedSnapArea)
            {
                AddMergedSnapArea();

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

        #endregion Design Mode
    }
}