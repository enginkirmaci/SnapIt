using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace SnapIt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MouseHook mh;

        public MainWindow()
        {
            InitializeComponent();

            mh = new MouseHook();
            mh.SetHook();
            mh.MouseMoveEvent += mh_MouseMoveEvent;
            mh.MouseDownEvent += mh_MouseDownEvent;
            mh.MouseUpEvent += mh_MouseUpEvent;
            mh.MouseClickEvent += Mh_MouseClickEvent;

            window = new Window1();
            window.Topmost = true;

            this.Closed += MainWindow_Closed;
        }

        private void Mh_MouseClickEvent(object sender, MouseEventArgs e)
        {
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            mh.UnHook();
            window.Close();
        }

        private IntPtr ActiveWindow;
        private Window1 window;
        private Rect newRect;
        private bool pressed = false;

        private void mh_MouseMoveEvent(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine("mh_MouseMoveEvent");

            // if (ActiveWindow != IntPtr.Zero)
            if (pressed)
            {
                if (!window.IsVisible)
                {
                    window.Show();
                }

                newRect = window.SelectElementWithPoint(e.Location.X, e.Location.Y);
            }
        }

        private void mh_MouseDownEvent(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                pressed = true;
                Debug.WriteLine("DownEvent");

                ActiveWindow = IntPtr.Zero;
                newRect = Rect.Empty;
            }
        }

        private void mh_MouseUpEvent(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("UpEvent");

            if (e.Button == MouseButtons.Left)
            {
                pressed = false;
                window.Hide();

                ActiveWindow = Win32ApiTest.GetActiveWindow();

                if (ActiveWindow != IntPtr.Zero)
                {
                    if (newRect != Rect.Empty)
                    {
                        SendKeys.SendWait("{ESC}");

                        Win32Api.DwmGetWindowAttribute(
                            ActiveWindow,
                            Win32Api.DWMWINDOWATTRIBUTE.ExtendedFrameBounds,
                            out Win32Api.Rectangle withMargin,
                            Marshal.SizeOf(typeof(Win32Api.Rectangle)));

                        var windowStyle = Win32ApiTest.RemoveBorders(ActiveWindow);

                        if (!withMargin.Equals(default(Win32Api.Rectangle)))
                        {
                            var noMargin = Win32ApiTest.GetWindowRectangle(ActiveWindow);

                            var systemMargin = new Win32Api.Rectangle()
                            {
                                Left = withMargin.Left - noMargin.Left,
                                Top = withMargin.Top - noMargin.Top,
                                Right = noMargin.Right - withMargin.Right,
                                Bottom = noMargin.Bottom - withMargin.Bottom
                            };

                            newRect.X -= systemMargin.Left;
                            newRect.Y -= systemMargin.Top;
                            newRect.Width += systemMargin.Left + systemMargin.Right;
                            newRect.Height += systemMargin.Top + systemMargin.Bottom;

                            Win32ApiTest.MoveWindow(ActiveWindow, newRect);
                        }
                        else
                        {
                            Win32ApiTest.MoveWindow(ActiveWindow, newRect);
                        }

                        Win32ApiTest.RedrawBorders(ActiveWindow, windowStyle);
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<IntPtr, string> window in Win32ApiTest.GetOpenWindows())
            {
                IntPtr handle = window.Key;
                string title = window.Value;
                if (title.Contains("Visual Studio Code"))
                {
                    var res = Win32ApiTest.MoveWindow(handle, 100, 100, 300, 300);

                    Debug.WriteLine("{0}: {1}", handle, title);
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var current = Win32ApiTest.GetActiveWindow();
            Win32ApiTest.GetWindowRectangle(current);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();

            window.Show();
        }
    }
}