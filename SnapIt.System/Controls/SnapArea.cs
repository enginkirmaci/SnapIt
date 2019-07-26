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
		private readonly SolidColorBrush buttonBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
		private readonly SolidColorBrush solidBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));

		private readonly PackIcon splitVerticallyIcon = new PackIcon { Kind = PackIconKind.ArrowSplitVertical };
		private readonly PackIcon splitHorizontallyIcon = new PackIcon { Kind = PackIconKind.ArrowSplitHorizontal };
		private readonly PackIcon removeSnapAreaIcon = new PackIcon { Kind = PackIconKind.Remove };

		private StackPanel designPanel;

		public SnapGrid MainGrid { get; set; }
		public SnapArea ParentSnapArea { get; set; }
		public LayoutArea LayoutArea { get; set; }

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
			Child = MainGrid = new SnapGrid();
			Name = "SnapArea_" + new Random().Next().ToString();

			Background = backgroundBrush;
			BorderBrush = borderBrush;
			BorderThickness = new Thickness(1);
		}

		public void GetLayoutAreas(LayoutArea layoutArea)
		{
			MainGrid.GetLayoutAreas(layoutArea);
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
			BorderThickness = new Thickness(0);
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

			MainGrid.Children.Add(designPanel);

			IsMouseDirectlyOverChanged += SnapArea_IsMouseDirectlyOverChanged;
		}

		private void RemoveSnapArea_Click(object sender, RoutedEventArgs e)
		{
			ParentSnapArea.Child = ParentSnapArea.MainGrid = new SnapGrid();

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

		public void ApplyLayout(LayoutArea layoutArea, bool isDesignMode, SnapArea parent = null)
		{
			if (isDesignMode)
			{
				SetDesignMode(parent);
			}
			else
			{
				Background = solidBackgroundBrush;
				BorderBrush = borderBrush;
				BorderThickness = new Thickness(1);
			}

			if (layoutArea.Areas != null && layoutArea.Areas.Count > 0)
			{
				if (isDesignMode)
				{
					IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;
					MainGrid.Children.Remove(designPanel);
				}

				MainGrid.ApplyLayout(layoutArea, isDesignMode);

				foreach (var area in layoutArea.Areas)
				{
					var snapArea = new SnapArea();

					MainGrid.Children.Add(snapArea);
					MainGrid.SetColumnRow(snapArea, area.Column, area.Row);
					snapArea.ApplyLayout(area, isDesignMode, this);
				}
			}
		}

		private void SplitVertically_Click(object sender, RoutedEventArgs e)
		{
			IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;

			MainGrid.Split(this, designPanel, SplitDirection.Vertically);
		}

		private void SplitHorizantally_Click(object sender, RoutedEventArgs e)
		{
			IsMouseDirectlyOverChanged -= SnapArea_IsMouseDirectlyOverChanged;

			MainGrid.Split(this, designPanel, SplitDirection.Horizantally);
		}
	}
}