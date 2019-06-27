using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SnapIt.Entities;
using SnapIt.InteropServices;

namespace SnapIt.Hooks
{
    public class MouseHook
    {
        private Point point;
        private Point Point
        {
            get { return point; }
            set
            {
                if (!point.Equals(value))
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

        private int hHook;
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
        public User32.HookProc hProc;

        public MouseHook()
        {
            Point = new Point();
        }

        public int SetHook()
        {
            hProc = new User32.HookProc(MouseHookProc);
            hHook = User32.SetWindowsHookEx(WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
            return hHook;
        }

        public void UnHook()
        {
            User32.UnhookWindowsHookEx(hHook);
        }

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            if (nCode < 0)
            {
                return User32.CallNextHookEx(hHook, nCode, wParam, lParam);
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
                Point = MyMouseHookStruct.pt;
                return User32.CallNextHookEx(hHook, nCode, wParam, lParam);
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