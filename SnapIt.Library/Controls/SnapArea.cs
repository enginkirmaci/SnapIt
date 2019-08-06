using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using SnapIt.Library.Entities;
using Point = System.Windows.Point;

namespace SnapIt.Library.Controls
{
	public class SnapArea : Grid
	{
		private const int splitterTickness = 10;
		private const int splitterMargin = -5;
		private readonly SolidColorBrush splitterBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

		private readonly SolidColorBrush backgroundBrush = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
		private readonly SolidColorBrush transparentBackgroundBrush = new SolidColorBrush(Colors.Transparent);
		private readonly SolidColorBrush borderBrush = new SolidColorBrush(Color.FromArgb(180, 200, 200, 200));
		private readonly SolidColorBrush backgroundOnHoverBrush = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0));
		private readonly SolidColorBrush buttonBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
		private readonly SolidColorBrush solidBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

		private readonly PackIcon splitVerticallyIcon = new PackIcon { Kind = PackIconKind.ArrowSplitVertical };
		private readonly PackIcon splitHorizontallyIcon = new PackIcon { Kind = PackIconKind.ArrowSplitHorizontal };
		private readonly PackIcon removeSnapAreaIcon = new PackIcon { Kind = PackIconKind.Remove };

		private StackPanel designPanel;

		public Border Border { get; set; }
		public SnapArea ParentSnapArea { get; set; }
		public LayoutArea LayoutArea { get; set; }
		public SplitDirection SplitDirection { get; private set; }

		public static readonly DependencyProperty LayoutAreaProperty
			= DependencyProperty.Register("LayoutArea", typeof(LayoutArea), typeof(SnapArea),
			  new FrameworkPropertyMetadata()
			  {
				  BindsTwoWayByDefault = true,
				  PropertyChangedCallback = new PropertyChangedCallback(LayoutAreaPropertyChanged)
			  });

		private static void LayoutAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((SnapArea)d).ApplyLayout((LayoutArea)e.NewValue, false);
		}

		public SnapArea()
		{
			Background = backgroundBrush;

			Border = new Border
			{
				BorderBrush = borderBrush,
				BorderThickness = new Thickness(1)
			};

			Children.Add(Border);
		}

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

		public void GenerateSnapAreaBoundries(ref List<Rectangle> rectangles)
		{
			var hasSnapChild = false;
			foreach (var child in Children)
			{
				if (child is SnapArea)
				{
					hasSnapChild = true;

					(child as SnapArea).GenerateSnapAreaBoundries(ref rectangles);
				}
			}

			if (!hasSnapChild)
			{
				rectangles.Add(ScreenSnapArea());
			}
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
			Border.BorderThickness = new Thickness(0);
			Border.Background = new SolidColorBrush(Colors.Transparent);

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

			SetZIndex(designPanel, 99);
			Children.Add(designPanel);

			Border.IsMouseDirectlyOverChanged += SnapArea_IsMouseDirectlyOverChanged;
		}

		private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
		{
			ParentSnapArea.Children.RemoveRange(0, ParentSnapArea.Children.Count);
			ParentSnapArea.Border = new Border
			{
				BorderBrush = borderBrush,
				BorderThickness = new Thickness(1)
			};

			ParentSnapArea.Children.Add(ParentSnapArea.Border);

			ParentSnapArea.SetDesignMode(ParentSnapArea.ParentSnapArea);

			if (ParentSnapArea.ColumnDefinitions.Count > 0)
			{
				ParentSnapArea.ColumnDefinitions.RemoveRange(0, ParentSnapArea.ColumnDefinitions.Count);
			}
			if (ParentSnapArea.RowDefinitions.Count > 0)
			{
				ParentSnapArea.RowDefinitions.RemoveRange(0, ParentSnapArea.RowDefinitions.Count);
			}
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

		public void ApplyLayout(LayoutArea layoutArea, bool isDesignMode, bool isTransparent = false, SnapArea parent = null)
		{
			if (isDesignMode)
			{
				SetDesignMode(parent);
			}
			else
			{
				Background = solidBackgroundBrush;
				Border.BorderBrush = borderBrush;
				Border.BorderThickness = new Thickness(1);

				if (isTransparent)
				{
					Background = backgroundBrush;
				}
			}

			if (parent != null)
			{
				parent.Background = transparentBackgroundBrush;
			}

			if (layoutArea?.Areas != null && layoutArea.Areas.Count > 0)
			{
				if (isDesignMode)
				{
					Border.IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;
					Children.Remove(designPanel);
				}

				ApplyLayout2(layoutArea, isDesignMode);

				foreach (var area in layoutArea.Areas)
				{
					var snapArea = new SnapArea();

					Children.Add(snapArea);
					SetColumnRow(snapArea, area.Column, area.Row);
					snapArea.ApplyLayout(area, isDesignMode, isTransparent, this);
				}
			}
		}

		internal void ApplyLayout2(LayoutArea layoutArea, bool isDesignMode)
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

		private void SplitVertically_Click(object sender, RoutedEventArgs e)
		{
			Border.IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;

			Split(this, designPanel, SplitDirection.Vertically);
		}

		private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
		{
			Border.IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;

			Split(this, designPanel, SplitDirection.Horizantally);
		}

		private void SetColumnRow(UIElement element, int column, int row)
		{
			SetColumn(element, column);
			SetRow(element, row);
		}

		private void Split(SnapArea snapArea, StackPanel designPanel, SplitDirection splitDirection)
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