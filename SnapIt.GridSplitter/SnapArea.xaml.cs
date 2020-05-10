using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SnapIt.GridSplitter.Entities;

namespace SnapIt.GridSplitter
{
    /// <summary>
    /// Interaction logic for SnapArea.xaml
    /// </summary>
    public partial class SnapArea : UserControl
    {
        public LayoutArea LayoutArea
        {
            get => (LayoutArea)GetValue(LayoutAreaProperty);
            set => SetValue(LayoutAreaProperty, value);
        }

        public static readonly DependencyProperty LayoutAreaProperty
            = DependencyProperty.Register("LayoutArea", typeof(LayoutArea), typeof(SnapArea),
              new FrameworkPropertyMetadata()
              {
                  BindsTwoWayByDefault = true,
                  PropertyChangedCallback = new PropertyChangedCallback(LayoutAreaPropertyChanged)
              });

        private static void LayoutAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var snapArea = (SnapArea)d;
            snapArea.LayoutArea = (LayoutArea)e.NewValue;
        }

        private static SnapArea Root { get; set; }

        public bool HasMergedSnapArea { get; set; }
        public bool IsMergedSnapArea { get; set; }
        //public LayoutArea LayoutArea { get; set; }
        public SplitDirection SplitDirection { get; private set; }

        public List<LayoutArea> Areas { get; set; }

        public SnapArea()
        {
            InitializeComponent();

            if (Root == null)
            {
                Root = this;
            }

            DesignPanel.Visibility = Visibility.Hidden;
        }

        public void ApplyLayout()
        {
            Root.Area.Children.Clear();

            if (Areas != null && Areas.Count > 0)
            {
                ApplyColumnsAndRows();

                FindAndFixMissingAreas();

                foreach (var area in Areas)
                {
                    AddSnapArea(area);
                }
            }
            else
            {
                SetDesignMode();
            }
        }

        private void FindAndFixMissingAreas()
        {
            var highestColumn = Areas.Max(area => area.Column);
            var highestRow = Areas.Max(area => area.Row);

            for (var j = 0; j <= highestRow; j++)
            {
                for (var i = 0; i <= highestColumn; i++)
                {
                    var found = Areas.FirstOrDefault(a => a.IsAt(i, j));

                    if (found == null)
                    {
                        found = Areas.FirstOrDefault(a => a.IsAt(i - 1, j));
                        if (found != null)
                        {
                            var testArea = found.Copy();
                            testArea.ColumnSpan++;
                            var hasCollision = Areas.Where(a => a != found).Any(a => a.HasCollision(testArea));

                            if (!hasCollision)
                            {
                                found.ColumnSpan++;
                            }
                            else
                            {
                                found = null;
                            }
                        }
                    }

                    if (found == null)
                    {
                        found = Areas.FirstOrDefault(a => a.IsAt(i, j - 1));
                        if (found != null)
                        {
                            var testArea = found.Copy();
                            testArea.RowSpan++;
                            var hasCollision = Areas.Where(a => a != found).Any(a => a.HasCollision(testArea));

                            if (!hasCollision)
                            {
                                found.RowSpan++;
                            }
                            else
                            {
                                found = null;
                            }
                        }
                    }

                    if (found == null)
                    {
                        found = Areas.FirstOrDefault(a => a.IsAt(i + 1, j));
                        if (found != null)
                        {
                            var testArea = found.Copy();
                            testArea.Column--;
                            var hasCollision = Areas.Where(a => a != found).Any(a => a.HasCollision(testArea));

                            if (!hasCollision)
                            {
                                found.Column--;
                            }
                            else
                            {
                                found = null;
                            }
                        }
                    }

                    if (found == null)
                    {
                        found = Areas.FirstOrDefault(a => a.IsAt(i, j + 1));
                        if (found != null)
                        {
                            var testArea = found.Copy();
                            testArea.Row--;
                            var hasCollision = Areas.Where(a => a != found).Any(a => a.HasCollision(testArea));

                            if (!hasCollision)
                            {
                                found.Row--;
                            }
                            else
                            {
                                found = null;
                            }
                        }
                    }

                    if (found == null)
                    {
                        Areas.Add(new LayoutArea
                        {
                            Column = i,
                            Row = j,
                            ColumnSpan = 1,
                            RowSpan = 1
                        });
                    }
                }
            }
        }

        internal void ApplyColumnsAndRows()
        {
            Area.ColumnDefinitions.Clear();
            Area.RowDefinitions.Clear();

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

        public void SetDesignMode()
        {
            if (this != Root)
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
            Split(SplitDirection.Vertically);
        }

        private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
        {
            Split(SplitDirection.Horizontally);
        }

        private SnapArea AddSnapArea(LayoutArea area)
        {
            var snapArea = new SnapArea
            {
                LayoutArea = area
            };
            snapArea.SetDesignMode();

            Root.Area.Children.Add(snapArea);
            SetColumnRow(snapArea, area.Column, area.Row, area.ColumnSpan, area.RowSpan);

            return snapArea;
        }

        private void Split(SplitDirection splitDirection)
        {
            SplitDirection = splitDirection;

            var originalLayout = LayoutArea.Copy();

            switch (SplitDirection)
            {
                case SplitDirection.Vertically:
                    var verticalLayout = LayoutArea.Copy();

                    Root.Areas.Add(verticalLayout);

                    if (LayoutArea.ColumnSpan > 1)
                    {
                        LayoutArea.ColumnSpan /= 2;
                        verticalLayout.ColumnSpan -= LayoutArea.ColumnSpan;
                        verticalLayout.Column += LayoutArea.ColumnSpan;
                    }
                    else
                    {
                        var horizontalAfter = new LayoutArea
                        {
                            Column = LayoutArea.Column + 1,
                            ColumnSpan = Root.Areas.Max(a => a.Column) + 1,
                            Row = originalLayout.Row,
                            RowSpan = originalLayout.RowSpan
                        };
                        var verticalMiddle = new LayoutArea
                        {
                            Column = originalLayout.Column,
                            ColumnSpan = originalLayout.ColumnSpan,
                            Row = 0,
                            RowSpan = Root.Areas.Max(a => a.Row) + 1
                        };
                        var horizontalintersects = Root.Areas.Where(a => a.HasCollision(horizontalAfter));
                        var verticalIntersects = Root.Areas.Where(a => a.HasCollision(verticalMiddle));

                        verticalLayout.Column++;

                        foreach (var area in Root.Areas)
                        {
                            if (area != verticalLayout &&
                                area.Column > originalLayout.Column && area.Column < Root.Areas.Max(a => a.Column) + 1)
                            {
                                area.Column++;
                            }
                        }

                        foreach (var area in verticalIntersects)
                        {
                            if (area != LayoutArea && area != verticalLayout)
                            {
                                area.ColumnSpan++;
                            }
                        }
                    }

                    Root.ApplyLayout();

                    break;

                case SplitDirection.Horizontally:
                    var horizontalLayout = LayoutArea.Copy();

                    Root.Areas.Add(horizontalLayout);

                    if (LayoutArea.RowSpan > 1)
                    {
                        LayoutArea.RowSpan /= 2;
                        horizontalLayout.RowSpan -= LayoutArea.RowSpan;
                        horizontalLayout.Row += LayoutArea.RowSpan;
                    }
                    else
                    {
                        var horizontalMiddle = new LayoutArea
                        {
                            Column = 0,
                            ColumnSpan = Root.Areas.Max(a => a.Column) + 1,
                            Row = originalLayout.Row,
                            RowSpan = originalLayout.RowSpan
                        };
                        var verticalAfter = new LayoutArea
                        {
                            Column = originalLayout.Column,
                            ColumnSpan = originalLayout.ColumnSpan,
                            Row = LayoutArea.Row + 1,
                            RowSpan = Root.Areas.Max(a => a.Row) + 1
                        };
                        var horizontalintersects = Root.Areas.Where(a => a.HasCollision(horizontalMiddle));
                        var verticalIntersects = Root.Areas.Where(a => a.HasCollision(verticalAfter));

                        horizontalLayout.Row++;

                        foreach (var area in Root.Areas)
                        {
                            if (area != horizontalLayout &&
                                area.Row > originalLayout.Row && area.Row < Root.Areas.Max(a => a.Row) + 1)
                            {
                                area.Row++;
                            }
                        }

                        foreach (var area in horizontalintersects)
                        {
                            if (area != LayoutArea && area != horizontalLayout)
                            {
                                area.RowSpan++;
                            }
                        }
                    }

                    Root.ApplyLayout();
                    break;
            }
        }

        private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
        {
            Root.Areas.Remove(LayoutArea);

            Root.ApplyLayout();
        }

        #endregion Design Mode
    }
}