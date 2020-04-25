using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;

namespace SnapIt.Test
{
    /// <summary>
    /// Interaction logic for SnapArea.xaml
    /// </summary>
    public partial class SnapArea : UserControl
    {
        public bool HasMergedSnapArea { get; set; }
        public bool IsMergedSnapArea { get; set; }
        public SnapArea ParentSnapArea { get; set; }
        public LayoutArea LayoutArea { get; set; }
        public SplitDirection SplitDirection { get; private set; }

        public bool IsDesignMode
        {
            get => (bool)GetValue(IsDesignModeProperty);
            set => SetValue(IsDesignModeProperty, value);
        }

        public static readonly DependencyProperty IsDesignModeProperty
         = DependencyProperty.Register("IsDesignMode", typeof(bool), typeof(SnapArea),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 PropertyChangedCallback = new PropertyChangedCallback(new PropertyChangedCallback(IsDesignModePropertyChanged))
             });

        private static void IsDesignModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapArea = (SnapArea)d;
            snapArea.IsDesignMode = (bool)e.NewValue;
        }

        public List<LayoutArea> Areas { get; set; }

        public SnapArea()
        {
            InitializeComponent();

            DesignPanel.Visibility = Visibility.Hidden;
            VerticalSplitter.Visibility = Visibility.Hidden;
            HorizantalSplitter.Visibility = Visibility.Hidden;
        }

        public void ApplyLayout(SnapArea parent = null)
        {
            if (Areas != null && Areas.Count > 0)
            {
                ApplyColumnsAndRows();

                foreach (var area in Areas)
                {
                    AddSnapArea(this, area);

                    break;
                }
            }
            else if (IsDesignMode)
            {
                SetDesignMode(parent);
            }
        }

        internal void ApplyColumnsAndRows()
        {
            if (Areas.Count > 0)
            {
                var highestColumn = Areas.Max(area => area.Column);
                var highestRow = Areas.Max(area => area.Row);

                for (int i = 0; i < highestColumn + 1; i++)
                {
                    if (highestColumn != 0)
                    {
                        Area.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                }

                for (int j = 0; j < highestRow + 1; j++)
                {
                    if (highestRow != 0)
                    {
                        Area.RowDefinitions.Add(new RowDefinition());
                    }
                }

                foreach (var area in Areas)
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

        private void SetColumnRow(UIElement element, int column, int row, int columnSpan, int rowSpan)
        {
            Grid.SetColumn(element, column);
            Grid.SetRow(element, row);
            Grid.SetColumnSpan(element, columnSpan);
            Grid.SetRowSpan(element, rowSpan);
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

        public void SetDesignMode(SnapArea parent)
        {
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
            Split(this, SplitDirection.Vertically);
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
            Split(this, SplitDirection.Horizantally);
        }

        private SnapArea AddSnapArea(SnapArea root, LayoutArea area)
        {
            var snapArea = new SnapArea
            {
                IsDesignMode = IsDesignMode,
                ParentSnapArea = root,
                LayoutArea = area
            };
            snapArea.SetDesignMode(this);

            root.Area.Children.Add(snapArea);
            SetColumnRow(snapArea, area.Column, area.Row, area.ColumnSpan, area.RowSpan);

            return snapArea;
        }

        private void Split(SnapArea snapArea, SplitDirection splitDirection)
        {
            SplitDirection = splitDirection;

            switch (SplitDirection)
            {
                case SplitDirection.Vertically:
                    var newLayout = snapArea.LayoutArea;
                    newLayout.Column++;
                    var newSnapArea = AddSnapArea(ParentSnapArea, snapArea.LayoutArea);

                    break;

                case SplitDirection.Horizantally:

                    break;
            }
        }

        private void AddGridSplitter(SplitDirection direction, int column, int row)
        {
            switch (direction)
            {
                case SplitDirection.Vertically:
                    VerticalSplitter.Visibility = Visibility.Visible;
                    Grid.SetColumn(VerticalSplitter, column);
                    Grid.SetRow(VerticalSplitter, row);

                    break;

                case SplitDirection.Horizantally:
                    HorizantalSplitter.Visibility = Visibility.Visible;
                    Grid.SetColumn(HorizantalSplitter, column);
                    Grid.SetRow(HorizantalSplitter, row);

                    break;
            }
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
            var childSnapAreas = ParentSnapArea.Area.FindChildren<SnapArea>();
            foreach (var child in childSnapAreas.ToList())
            {
                ParentSnapArea.Area.Children.Remove(child);
            }

            ParentSnapArea.Border.Visibility = Visibility.Visible;

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

        #endregion Design Mode
    }
}