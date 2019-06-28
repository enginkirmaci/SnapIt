using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SnapIt.Entities;
using SnapIt.Hooks;
using SnapIt.InteropServices;

namespace SnapIt.Services
{
    public class SnapService : ISnapService
    {
        private readonly IWindowService windowService;

        private MouseHook mouseHook;
        private SnapWindow snapWindow;
        private IntPtr ActiveWindow;
        private Rectangle ActiveWindowRectangle;
        private Rectangle snapArea;
        private bool isWindowDetected = false;
        private bool isListening = false;

        public SnapWindow SnapWindow
        {
            get
            {
                if (snapWindow == null)
                {
                    snapWindow = windowService.CreateSnapWindow();
                }

                return snapWindow;
            }
        }

        public SnapService(IWindowService windowService)
        {
            this.windowService = windowService;
        }

        public void Initialize()
        {
            mouseHook = new MouseHook();
            mouseHook.SetHook();
            mouseHook.MouseMoveEvent += MouseMoveEvent;
            mouseHook.MouseDownEvent += MouseDownEvent;
            mouseHook.MouseUpEvent += MouseUpEvent;
            mouseHook.MouseClickEvent += MouseClickEvent;
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

                    //check if mouse click on title bar
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
                else if (!SnapWindow.IsVisible)
                {
                    SnapWindow.Show();
                }
                else
                {
                    snapArea = SnapWindow.SelectElementWithPoint(e.Location.X, e.Location.Y);
                }
            }
        }

        private void MouseDownEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Debug.WriteLine("DownEvent");

                ActiveWindow = IntPtr.Zero;
                snapArea = Rectangle.Empty;
                isWindowDetected = false;
                isListening = true;
            }
        }

        private void MouseUpEvent(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("UpEvent");

            if (e.Button == MouseButtons.Left)
            {
                isListening = false;
                SnapWindow.Hide();

                if (ActiveWindow != IntPtr.Zero)
                {
                    if (!snapArea.Equals(Rectangle.Empty))
                    {
                        SendKeys.SendWait("{ESC}");

                        DwmApi.DwmGetWindowAttribute(
                            ActiveWindow,
                            DWMWINDOWATTRIBUTE.ExtendedFrameBounds,
                            out Rectangle withMargin,
                            Marshal.SizeOf(typeof(Rectangle)));

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