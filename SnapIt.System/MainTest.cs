using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SnapIt.Entities;
using SnapIt.Hooks;
using SnapIt.InteropServices;

namespace SnapIt
{
    public class MainTest
    {
        private MouseHook mh;

        public MainTest()
        {
            mh = new MouseHook();
            mh.SetHook();
            mh.MouseMoveEvent += mh_MouseMoveEvent;
            mh.MouseDownEvent += mh_MouseDownEvent;
            mh.MouseUpEvent += mh_MouseUpEvent;
            mh.MouseClickEvent += Mh_MouseClickEvent;

            //window = new WindowTest();
            //window.Topmost = true;
        }

        private void Mh_MouseClickEvent(object sender, MouseEventArgs e)
        {
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            mh.UnHook();
            //window.Close();
        }

        private IntPtr ActiveWindow;

        //private WindowTest window;
        private Rectangle newRect;

        private bool pressed = false;

        private void mh_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            //if (pressed)
            //{
            //    if (!window.IsVisible)
            //    {
            //        window.Show();
            //    }

            //    newRect = window.SelectElementWithPoint(e.Location.X, e.Location.Y);
            //}
        }

        private void mh_MouseDownEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pressed = true;
                Debug.WriteLine("DownEvent");

                ActiveWindow = IntPtr.Zero;
                newRect = Rectangle.Empty;
            }
        }

        private void mh_MouseUpEvent(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("UpEvent");

            if (e.Button == MouseButtons.Left)
            {
                pressed = false;
                //window.Hide();

                ActiveWindow = User32Test.GetActiveWindow();

                if (ActiveWindow != IntPtr.Zero)
                {
                    if (!newRect.Equals(Rectangle.Empty))
                    {
                        SendKeys.SendWait("{ESC}");

                        DwmApi.DwmGetWindowAttribute(
                            ActiveWindow,
                            DWMWINDOWATTRIBUTE.ExtendedFrameBounds,
                            out Rectangle withMargin,
                            Marshal.SizeOf(typeof(Rectangle)));

                        var windowStyle = User32Test.RemoveBorders(ActiveWindow);

                        if (!withMargin.Equals(default(Rectangle)))
                        {
                            var noMargin = User32Test.GetWindowRectangle(ActiveWindow);

                            var systemMargin = new Rectangle()
                            {
                                Left = withMargin.Left - noMargin.Left,
                                Top = withMargin.Top - noMargin.Top,
                                Right = noMargin.Right - withMargin.Right,
                                Bottom = noMargin.Bottom - withMargin.Bottom
                            };

                            newRect.Left -= systemMargin.Left;
                            newRect.Top -= systemMargin.Top;
                            newRect.Right += systemMargin.Right;
                            newRect.Bottom += systemMargin.Bottom;

                            User32Test.MoveWindow(ActiveWindow, newRect);
                        }
                        else
                        {
                            User32Test.MoveWindow(ActiveWindow, newRect);
                        }

                        User32Test.RedrawBorders(ActiveWindow, windowStyle);
                    }
                }
            }
        }
    }
}