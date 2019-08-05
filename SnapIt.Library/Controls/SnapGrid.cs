using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Controls
{
	public class SnapGrid : Grid
	{
		private const int splitterTickness = 10;
		private const int splitterMargin = -5;
		private readonly SolidColorBrush splitterBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

		public SplitDirection SplitDirection { get; private set; }

		public void GetLayoutAreas(LayoutArea layoutArea)
		{
			foreach (var child in Children)
			{
				if (child is SnapArea)
				{
					var childSnapArea = child as SnapArea;

					var column = GetColumn(childSnapArea);
					var row = GetRow(childSnapArea);

					var childLayoutArea = new LayoutArea
					{
						Column = column,
						Row = row,
						Width = ColumnDefinitions.Count > column ? (int?)ColumnDefinitions[column].Width.Value : null,
						Height = RowDefinitions.Count > row ? (int?)RowDefinitions[row].Height.Value : null
					};

					childSnapArea.GetLayoutAreas(childLayoutArea);

					if (layoutArea.Areas == null)
					{
						layoutArea.Areas = new List<LayoutArea>();
					}

					layoutArea.Areas.Add(childLayoutArea);
				}
			}
		}

		public void Split(SnapArea snapArea, StackPanel designPanel, SplitDirection splitDirection)
		{
			SplitDirection = splitDirection;
			Children.Remove(designPanel);

			var area1 = new SnapArea();
			var area2 = new SnapArea();
			area1.SetDesignMode(snapArea);
			area2.SetDesignMode(snapArea);

			Children.Add(area1);
			Children.Add(area2);

			switch (SplitDirection)
			{
				case SplitDirection.Vertically:

					ColumnDefinitions.Add(new ColumnDefinition());
					ColumnDefinitions.Add(new ColumnDefinition());

					SetColumn(area2, 1);
					AddGridSplitter(SplitDirection, 1, 0);

					break;

				case SplitDirection.Horizantally:

					RowDefinitions.Add(new RowDefinition());
					RowDefinitions.Add(new RowDefinition());

					SetRow(area2, 1);
					AddGridSplitter(SplitDirection, 0, 1);

					break;
			}
		}

		internal void SetColumnRow(UIElement element, int column, int row)
		{
			SetColumn(element, column);
			SetRow(element, row);
		}

		internal void ApplyLayout(LayoutArea layoutArea, bool isDesignMode)
		{
			if (layoutArea.Areas.Count > 0)
			{
				var highestColumn = layoutArea.Areas.Max(area => area.Column);
				var highestRow = layoutArea.Areas.Max(area => area.Row);

				for (int i = 0; i < highestColumn + 1; i++)
				{
					if (highestColumn != 0)
					{
						ColumnDefinitions.Add(new ColumnDefinition());
					}

					for (int j = 0; j < highestRow + 1; j++)
					{
						if (highestRow != 0)
						{
							RowDefinitions.Add(new RowDefinition());
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
					if (ColumnDefinitions.Count > 0 && area.Width.HasValue)
					{
						ColumnDefinitions[area.Column].Width = new GridLength(area.Width.Value, GridUnitType.Star);
					}

					if (RowDefinitions.Count > 0 && area.Height.HasValue)
					{
						RowDefinitions[area.Row].Height = new GridLength(area.Height.Value, GridUnitType.Star);
					}
				}
			}
		}

		private void AddGridSplitter(SplitDirection direction, int column, int row)
		{
			switch (direction)
			{
				case SplitDirection.Vertically:
					var verticalSplitter = new GridSplitter
					{
						Width = splitterTickness,
						Margin = new Thickness(splitterMargin, 0, 0, 0),
						HorizontalAlignment = HorizontalAlignment.Left,
						VerticalAlignment = VerticalAlignment.Stretch,
						ResizeDirection = GridResizeDirection.Auto,
						Background = splitterBackgroundBrush
					};
					Children.Add(verticalSplitter);

					SetZIndex(verticalSplitter, 10);
					SetColumn(verticalSplitter, column);
					SetRow(verticalSplitter, row);

					break;

				case SplitDirection.Horizantally:
					var horizantalSplitter = new GridSplitter
					{
						Height = splitterTickness,
						Margin = new Thickness(0, splitterMargin, 0, 0),
						HorizontalAlignment = HorizontalAlignment.Stretch,
						VerticalAlignment = VerticalAlignment.Top,
						ResizeDirection = GridResizeDirection.Auto,
						Background = splitterBackgroundBrush
					};

					Children.Add(horizantalSplitter);

					SetZIndex(horizantalSplitter, 10);
					SetColumn(horizantalSplitter, column);
					SetRow(horizantalSplitter, row);

					break;
			}
		}
	}
}