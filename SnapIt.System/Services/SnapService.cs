using System.Diagnostics;
using System.Windows.Forms;
using SnapIt.Configuration;
using SnapIt.Entities;
using SnapIt.Hooks;
using SnapIt.Mappers;

namespace SnapIt.Services
{
	public class SnapService : ISnapService
	{
		private readonly IWindowService windowService;
		private readonly ConfigService configService;

		private Config config;
		private MouseHook mouseHook;
		private ActiveWindow ActiveWindow;
		private Rectangle ActiveWindowRectangle;
		private Rectangle snapArea;
		private bool isWindowDetected = false;
		private bool isListening = false;

		public event GetStatus StatusChanged;

		public SnapService(
			IWindowService windowService,
			ConfigService configService)
		{
			this.windowService = windowService;
			this.configService = configService;
		}

		public void Initialize()
		{
			config = configService.Load<Config>();
			windowService.Initialize();

			mouseHook = new MouseHook();
			mouseHook.SetHook();
			mouseHook.MouseMoveEvent += MouseMoveEvent;
			mouseHook.MouseDownEvent += MouseDownEvent;
			mouseHook.MouseUpEvent += MouseUpEvent;
			mouseHook.MouseClickEvent += MouseClickEvent;

			StatusChanged?.Invoke(true);
		}

		public void Release()
		{
			config = null;
			windowService.Release();

			if (mouseHook != null)
			{
				mouseHook.MouseMoveEvent -= MouseMoveEvent;
				mouseHook.MouseDownEvent -= MouseDownEvent;
				mouseHook.MouseUpEvent -= MouseUpEvent;
				mouseHook.MouseClickEvent -= MouseClickEvent;
				mouseHook.UnHook();
				mouseHook = null;
			}

			StatusChanged?.Invoke(false);
		}

		private void MouseClickEvent(object sender, MouseEventArgs e)
		{
		}

		private void MouseMoveEvent(object sender, MouseEventArgs e)
		{
			if (isListening)
			{
				if (!isWindowDetected)
				{
					ActiveWindow = User32Test.GetActiveWindow();
					ActiveWindowRectangle = User32Test.GetWindowRectangle(ActiveWindow);

					var titleBarHeight = SystemInformation.CaptionHeight;
					var FixedFrameBorderSize = SystemInformation.FixedFrameBorderSize.Height;

					var res = User32Test.IsFullscreen(ActiveWindowRectangle);
					Debug.WriteLine(res);

					if (ActiveWindow != null && ActiveWindow.Title.Contains("Snap It"))
					{
						isListening = false;
					}
					//check if mouse click on title bar
					else
					if (config.DragByTitle)
					{
						if (ActiveWindowRectangle.Top + titleBarHeight + FixedFrameBorderSize * 2 >= e.Location.Y)
						{
							isWindowDetected = true;
							Debug.WriteLine("window detected");
						}
						else
						{
							isListening = false;
						}
					}
					else
					{
						isWindowDetected = true;
					}
				}
				else if (!windowService.IsVisible)
				{
					windowService.Show();
				}
				else
				{
					snapArea = windowService.SelectElementWithPoint(e.Location.X, e.Location.Y);
				}
			}
		}

		private void MouseDownEvent(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtonMapper.Map(config.MouseButton))
			{
				Debug.WriteLine("DownEvent");

				ActiveWindow = ActiveWindow.Empty;
				snapArea = Rectangle.Empty;
				isWindowDetected = false;
				isListening = true;
			}
		}

		private void MouseUpEvent(object sender, MouseEventArgs e)
		{
			Debug.WriteLine("UpEvent");

			if (e.Button == MouseButtonMapper.Map(config.MouseButton))
			{
				isListening = false;
				windowService.Hide();

				if (ActiveWindow != ActiveWindow.Empty)
				{
					if (!snapArea.Equals(Rectangle.Empty))
					{
						SendKeys.SendWait("{ESC}");

						User32Test.GetWindowMargin(ActiveWindow, out Rectangle withMargin);

						if (!withMargin.Equals(default(Rectangle)))
						{
							var systemMargin = new Rectangle()
							{
								Left = withMargin.Left - ActiveWindowRectangle.Left,
								Top = withMargin.Top - ActiveWindowRectangle.Top,
								Right = ActiveWindowRectangle.Right - withMargin.Right,
								Bottom = ActiveWindowRectangle.Bottom - withMargin.Bottom
							};

							snapArea.Left -= systemMargin.Left;
							snapArea.Top -= systemMargin.Top;
							snapArea.Right += systemMargin.Right;
							snapArea.Bottom += systemMargin.Bottom;

							User32Test.MoveWindow(ActiveWindow, snapArea);
						}
						else
						{
							User32Test.MoveWindow(ActiveWindow, snapArea);
						}
					}
				}
			}
		}
	}
}