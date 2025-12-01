using System.Runtime.InteropServices;
using System.Windows.Forms;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;
using SnapIt.Common.InteropServices;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class WinApiService : IWinApiService
{
    private const int MAX_PATH = 260;
    public delegate void WinEventDelegate(nint hWinEventHook, uint eventType, nint hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    public static extern nint SetWinEventHook(uint eventMin, uint eventMax, nint hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(nint hWinEventHook);

    public const uint EVENT_OBJECT_CREATE = 0x8000;
    public const uint EVENT_OBJECT_SHOW = 0x8002;
    public const uint EVENT_OBJECT_DESTROY = 0x8001;
    public const uint EVENT_OBJECT_HIDE = 0x8003;
    public const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    public const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;
    public const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;

    public const uint WINEVENT_OUTOFCONTEXT = 0x0000;
    public const uint WINEVENT_SKIPOWNTHREAD = 0x0001;
    public const uint WINEVENT_SKIPOWNPROCESS = 0x0002;
    public const uint WINEVENT_INCONTEXT = 0x0004;

    public const int OBJID_WINDOW = 0x00000000;

    private readonly ISettingService settingService;

    public bool IsInitialized { get; private set; }

    public WinApiService(ISettingService settingService)
    {
        this.settingService = settingService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        IsInitialized = true;
    }

    public IDictionary<nint, string> GetOpenWindows()
    {
        var shellWindow = PInvoke.User32.GetShellWindow();
        var windows = new Dictionary<nint, string>();

        PInvoke.User32.EnumWindows(delegate (nint hwnd, nint lParam)
        {
            if (hwnd == shellWindow) return true;
            if (!PInvoke.User32.IsWindowVisible(hwnd)) return true;

            var length = PInvoke.User32.GetWindowTextLength(hwnd);
            if (length == 0) return true;

            var text = new char[length + 1];
            var finalLength = PInvoke.User32.GetWindowText(hwnd, text, length + 1);
            if (finalLength == 0) return true;

            windows[hwnd] = new string(text, 0, finalLength);
            return true;
        }, nint.Zero);

        return windows;
    }

    public IEnumerable<string> GetOpenWindowsNames()
    {
        return GetOpenWindows().Select(i => i.Value).Distinct().OrderBy(i => i);
    }

    public bool IsAllowedWindowStyle(ActiveWindow activeWindow)
    {
        var style = PInvoke.User32.GetWindowLong(activeWindow.Handle, PInvoke.User32.WindowLongIndexFlags.GWL_STYLE);

        var windowStyle = (PInvoke.User32.WindowStylesEx)(style & (uint)PInvoke.User32.WindowStylesEx.WS_EX_APPWINDOW);

        return windowStyle == PInvoke.User32.WindowStylesEx.WS_EX_APPWINDOW;
    }

    public bool IsFullscreen(ActiveWindow activeWindow)
    {
        PInvoke.User32.GetWindowRect(PInvoke.User32.GetDesktopWindow(), out PInvoke.RECT desktopWindow);
        var isFullScreen = activeWindow.Boundry.Left == desktopWindow.left &&
                activeWindow.Boundry.Top == desktopWindow.top &&
                activeWindow.Boundry.Right == desktopWindow.right &&
                activeWindow.Boundry.Bottom == desktopWindow.bottom;

        return isFullScreen;
    }

    public void MoveWindow(ActiveWindow activeWindow, Rectangle newRect)
    {
        MoveWindow(activeWindow, (int)newRect.X, (int)newRect.Y, (int)newRect.Width, (int)newRect.Height);
    }

    public void MoveWindow(ActiveWindow activeWindow, int X, int Y, int width, int height)
    {
        if (activeWindow == null) return;

        Dev.Log($"{activeWindow.Handle}, {X},{Y}  {width}x{height}");

        PInvoke.User32.ShowWindow(activeWindow.Handle, PInvoke.User32.WindowShowStyle.SW_SHOWNORMAL);

        var res = PInvoke.User32.SetWindowPos(
            activeWindow.Handle,
            PInvoke.User32.SpecialWindowHandles.HWND_TOP,
            X,
            Y,
            width,
            height,
            PInvoke.User32.SetWindowPosFlags.SWP_SHOWWINDOW | PInvoke.User32.SetWindowPosFlags.SWP_ASYNCWINDOWPOS);

        var msg = Marshal.GetLastWin32Error();
        if (msg != 0)
        {
            Dev.Log(msg);
        }
    }

    public void SendMessage(ActiveWindow activeWindow)
    {
        PInvoke.User32.SendMessage(activeWindow.Handle, PInvoke.User32.WindowMessage.WM_KEYDOWN, (nint)Keys.Escape, (nint)0);
    }

    public void GetWindowMargin(ActiveWindow activeWindow, out Rectangle withMargin)
    {
        if (activeWindow != null)
        {
            var t = new PInvoke.RECT();
            DwmApi.DwmGetWindowAttribute(
                            activeWindow.Handle,
                            DWMWINDOWATTRIBUTE.ExtendedFrameBounds,
                            out t,
                            Marshal.SizeOf(typeof(PInvoke.RECT)));

            Dev.Log(t.ToString());

            withMargin = new Rectangle(t.left, t.top, t.right, t.bottom);
        }
        else
        {
            withMargin = Rectangle.Empty;
        }
    }

    public ActiveWindow GetActiveWindow()
    {
        var activeWindow = new ActiveWindow
        {
            Handle = PInvoke.User32.GetForegroundWindow()
        };

        var chars = 256;
        var buff = new char[chars + 1];
        var length = PInvoke.User32.GetWindowText(activeWindow.Handle, buff, chars);
        if (length > 0)
        {
            activeWindow.Title = new string(buff, 0, length);
        }

        if (PInvoke.User32.GetWindowRect(activeWindow.Handle, out PInvoke.RECT rct))
        {
            activeWindow.Boundry = new Rectangle(rct.left, rct.top, rct.right, rct.bottom);
        }

        if (activeWindow.Handle == nint.Zero || activeWindow.Boundry.Equals(Rectangle.Empty))
            activeWindow = ActiveWindow.Empty;

        return activeWindow;
    }

    public string GetCurrentDesktopWallpaper()
    {
        var buff = new char[MAX_PATH];

        nint ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buff, 0);

        PInvoke.User32.SystemParametersInfo(
            PInvoke.User32.SystemParametersInfoAction.SPI_GETDESKWALLPAPER,
            (uint)buff.Length,
            ptr,
            PInvoke.User32.SystemParametersInfoFlags.None);

        var currentWallpaper = new string(buff, 0, buff.Length);
        return currentWallpaper.Substring(0, currentWallpaper.IndexOf('\0'));
    }

    public void SetWindowCornerPreference(ActiveWindow activeWindow, DWM_WINDOW_CORNER_PREFERENCE preference)
    {
        if (activeWindow == null || activeWindow.Handle == nint.Zero)
            return;

        try
        {
            int pref = (int)preference;
            DwmApi.DwmSetWindowAttribute(
                activeWindow.Handle,
                DWMWINDOWATTRIBUTE.WindowCornerPreference,
                ref pref,
                sizeof(int));
        }
        catch (Exception ex)
        {
            Dev.Log($"Failed to set window corner preference: {ex.Message}");
        }
    }

    public void Dispose()
    {
        IsInitialized = false;
    }
}

