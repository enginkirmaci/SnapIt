using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SnapIt.Library.Entities;
using Point = System.Windows.Point;

namespace SnapIt.Library.Controls
{
	public class SnapWindow : Window
	{
		private SnapArea current;

		public SnapScreen Screen { get; set; }
		public List<Rectangle> SnapAreaBoundries { get; set; }

		//private double DpiX = 1.0;
		//private double DpiY = 1.0;

		public SnapWindow(SnapScreen screen)
		{
			Screen = screen;

			//CalculateDpi();

			Topmost = true;
			AllowsTransparency = true;
			Background = new SolidColorBrush(Colors.Transparent);
			ResizeMode = ResizeMode.NoResize;
			ShowInTaskbar = false;
			Width = screen.Base.WorkingArea.Width;
			Height = screen.Base.WorkingArea.Height;
			Left = screen.Base.WorkingArea.X;
			Top = screen.Base.WorkingArea.Y;
			WindowState = WindowState.Normal;
			WindowStyle = WindowStyle.None;
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var wih = new WindowInteropHelper(this);

			var window = new ActiveWindow
			{
				Handle = wih.Handle
			};

			User32Test.MoveWindow(
				window,
				Screen.Base.WorkingArea.Left,
				Screen.Base.WorkingArea.Top,
				Screen.Base.WorkingArea.Width,
				Screen.Base.WorkingArea.Height);
		}

		//private void CalculateDpi()
		//{
		//    screen.Base.GetDpi(DpiType.Effective, out uint x, out uint y);

		//    DpiX = 96.0 / x;
		//    DpiY = 96.0 / y;
		//}

		public void ApplyLayout()
		{
			var snapArea = new SnapArea();

			if (Screen.Layout != null)
			{
				snapArea.ApplyLayout(Screen.Layout.LayoutArea, false, true);
			}

			Content = snapArea;
		}

		public void GenerateSnapAreaBoundries()
		{
			if (SnapAreaBoundries == null)
			{
				var generated = new List<Rectangle>();

				var rootSnapArea = Content as SnapArea;
				rootSnapArea.GenerateSnapAreaBoundries(ref generated);

				SnapAreaBoundries = generated.OrderBy(i => i.X).ThenBy(i => i.Y).ToList();
			}
		}

		public Rectangle SelectElementWithPoint(int x, int y)
		{
			if (IsVisible)
			{
				var Point2Window = PointFromScreen(new Point(x, y));

				var element = InputHitTest(Point2Window);
				if (element != null && element is SnapArea)
				{
					if (current != null)
					{
						current.Background = new SolidColorBrush(Color.FromArgb(25, 255, 255, 255));
					}

					var snapArea = current = (SnapArea)element;

					snapArea.OnHoverStyle();

					return snapArea.ScreenSnapArea();
				}
				else
				{
					//TODO imporove here. moving on different screens, old one preserves the hover style
					if (current != null)
					{
						current.NormalStyle();
					}
				}
			}

			return Rectangle.Empty;
		}
	}
}