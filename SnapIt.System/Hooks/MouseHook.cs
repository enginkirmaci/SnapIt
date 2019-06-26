using PInvoke;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnapIt.System.Hooks
{
    public class MouseHook
    {
        private Point point;
        private Point Point
        {
            get { return point; }
            set
            {
                if (point != value)
                {
                    point = value;
                    if (MouseMoveEvent != null)
                    {
                        var e = new MouseEventArgs(MouseButtons.None, 0, point.X, point.Y, 0);
                        MouseMoveEvent(this, e);
                    }
                }
            }
        }

        private User32.SafeHookHandle hHook;
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;
        public const int WH_MOUSE_LL = 14;
        public User32.WindowsHookDelegate hProc;

        public MouseHook()
        {
            this.Point = new Point();
        }

        public User32.SafeHookHandle SetHook()
        {
            hProc = new User32.WindowsHookDelegate(MouseHookProc);
            hHook = User32.SetWindowsHookEx(User32.WindowsHookType.WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
            return hHook;
        }

        public void UnHook()
        {
            hHook.Dispose();
        }

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            User32.MouseHookStruct MyMouseHookStruct = (Win32Api.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.MouseHookStruct));
            if (nCode < 0)
            {
                return User32.CallNextHookEx(hHook.DangerousGetHandle(), nCode, wParam, lParam); //TODO change DangerousGetHandle
            }
            else
            {
                if (MouseClickEvent != null)
                {
                    MouseButtons button = MouseButtons.None;
                    int clickCount = 0;
                    switch ((Int32)wParam)
                    {
                        case WM_LBUTTONDOWN:
                            button = MouseButtons.Left;
                            clickCount = 1;
                            MouseDownEvent(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0));
                            break;

                        case WM_RBUTTONDOWN:
                            button = MouseButtons.Right;
                            clickCount = 1;
                            MouseDownEvent(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0));
                            break;

                        case WM_MBUTTONDOWN:
                            button = MouseButtons.Middle;
                            clickCount = 1;
                            MouseDownEvent(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0));
                            break;

                        case WM_LBUTTONUP:
                            button = MouseButtons.Left;
                            clickCount = 1;
                            MouseUpEvent(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0));
                            break;

                        case WM_RBUTTONUP:
                            button = MouseButtons.Right;
                            clickCount = 1;
                            MouseUpEvent(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0));
                            break;

                        case WM_MBUTTONUP:
                            button = MouseButtons.Middle;
                            clickCount = 1;
                            MouseUpEvent(this, new MouseEventArgs(button, clickCount, point.X, point.Y, 0));
                            break;
                    }

                    var e = new MouseEventArgs(button, clickCount, point.X, point.Y, 0);
                    MouseClickEvent(this, e);
                }
                this.Point = new Point(MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y);
                return User32.CallNextHookEx(hHook.DangerousGetHandle(), nCode, wParam, lParam);  //TODO change DangerousGetHandle
            }
        }

        public delegate void MouseMoveHandler(object sender, MouseEventArgs e);

        public event MouseMoveHandler MouseMoveEvent;

        public delegate void MouseClickHandler(object sender, MouseEventArgs e);

        public event MouseClickHandler MouseClickEvent;

        public delegate void MouseDownHandler(object sender, MouseEventArgs e);

        public event MouseDownHandler MouseDownEvent;

        public delegate void MouseUpHandler(object sender, MouseEventArgs e);

        public event MouseUpHandler MouseUpEvent;
    }
}