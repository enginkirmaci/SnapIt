using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using SnapIt.Common.Entities;
using SnapIt.Common.Events;
using SnapIt.Common.Extensions;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class MouseService : IMouseService
{
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private readonly IWindowsService windowsService;
    private IKeyboardMouseEvents globalHook;

    private bool isWindowDetected = false;
    private bool isListening = false;
    private bool isHoldingKey = false;
    private bool holdKeyUsed = false;
    private ActiveWindow activeWindow;
    private SnapAreaInfo? snapAreaInfo;
    private System.Drawing.Point startLocation;

    public bool IsInitialized { get; private set; }

    public event HideWindowsEvent HideWindows;

    public event MoveWindowEvent MoveWindow;

    public event SnappingCancelEvent SnappingCancelled;

    public event ShowWindowsIfNecessaryEvent ShowWindowsIfNecessary;

    public event SelectElementWithPointEvent SelectElementWithPoint;

    public MouseService(
        ISettingService settingService,
        IWinApiService winApiService,
        IWindowsService windowsService)
    {
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.windowsService = windowsService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        await settingService.InitializeAsync();
        await winApiService.InitializeAsync();
        await windowsService.InitializeAsync();

        globalHook = Hook.GlobalEvents();

        if (settingService.Settings.EnableMouse)
        {
            globalHook.MouseMove += MouseMoveEvent;
            globalHook.MouseDown += MouseDownEvent;
            globalHook.MouseUp += MouseUpEvent;

            if (settingService.Settings.EnableHoldKey)
            {
                globalHook.KeyDown += GlobalHook_KeyDown;
                globalHook.KeyUp += GlobalHook_KeyUp;
            }
        }

        isWindowDetected = false;
        isListening = false;

        IsInitialized = true;
    }

    public void Dispose()
    {
        if (globalHook != null)
        {
            globalHook.MouseMove -= MouseMoveEvent;
            globalHook.MouseDown -= MouseDownEvent;
            globalHook.MouseUp -= MouseUpEvent;

            if (settingService.Settings.EnableHoldKey)
            {
                globalHook.KeyDown -= GlobalHook_KeyDown;
                globalHook.KeyUp -= GlobalHook_KeyUp;
            }
        }

        IsInitialized = false;
    }

    public void Interrupt()
    {
        isListening = false;
    }

    private void MouseMoveEvent(object sender, MouseEventArgs e)
    {
        if (isListening)
        {
            var p = WpfScreenHelper.MouseHelper.MousePosition;

            if (HoldingKeyResult() && IsDelayDone(p))
            {
                if (!isWindowDetected)
                {
                    holdKeyUsed = true;

                    activeWindow = winApiService.GetActiveWindow();
                    activeWindow.Dpi = DpiHelper.GetDpiFromPoint((int)p.X, (int)p.Y);

                    if (activeWindow?.Title != null && windowsService.IsExcludedApplication(activeWindow.Title, false))
                    {
                        isListening = false;
                    }
                    else if (settingService.Settings.DisableForFullscreen && winApiService.IsFullscreen(activeWindow))
                    {
                        isListening = false;
                    }
                    else if (settingService.Settings.DisableForModal && !winApiService.IsAllowedWindowStyle(activeWindow))
                    {
                        isListening = false;
                    }
                    else if (settingService.Settings.DragByTitle)
                    {
                        var titleBarHeight = SystemInformation.CaptionHeight;
                        var FixedFrameBorderSize = SystemInformation.FixedFrameBorderSize.Height;

                        if (activeWindow.Boundry.Top + titleBarHeight + 2 + FixedFrameBorderSize * 2 >= p.Y)
                        {
                            isWindowDetected = true;
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
                else if (ShowWindowsIfNecessary != null && ShowWindowsIfNecessary.Invoke())
                {
                    //todo maybe ShowWindowsIfNecessary?.Invoke() can work before SelectElementWithPoint
                }
                else
                {
                    snapAreaInfo = SelectElementWithPoint?.Invoke((int)p.X, (int)p.Y);

                    if (snapAreaInfo?.Screen != null)
                    {
                        settingService.LatestActiveScreen = snapAreaInfo.Screen;
                    }
                }
            }
        }
    }

    private void MouseDownEvent(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtonsMap(settingService.Settings.MouseButton))
        {
            activeWindow = ActiveWindow.Empty;
            snapAreaInfo = SnapAreaInfo.Empty;
            isWindowDetected = false;
            isListening = true;

            startLocation = e.Location;
        }
    }

    private void MouseUpEvent(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtonsMap(settingService.Settings.MouseButton) && isListening)
        {
            isListening = false;
            HideWindows?.Invoke();

            MoveWindow?.Invoke(new SnapAreaInfo
            {
                ActiveWindow = activeWindow,
                Rectangle = snapAreaInfo?.Rectangle
            }, e.Button == MouseButtons.Left);
        }
    }

    private void GlobalHook_KeyUp(object sender, KeyEventArgs e)
    {
        switch (settingService.Settings.HoldKey)
        {
            case HoldKey.Control:
                if (e.KeyCode == Keys.Control || e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Alt:
                if (e.KeyCode == Keys.Alt || e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Shift:
                if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Win:
                if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                {
                    isHoldingKey = false;

                    if (holdKeyUsed)
                    {
                        e.Handled = true;
                    }
                }

                break;
        }

        if (holdKeyUsed)
        {
            holdKeyUsed = false;
        }
    }

    private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
    {
        switch (settingService.Settings.HoldKey)
        {
            case HoldKey.Control:
                if (e.KeyCode == Keys.Control || e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Alt:
                if (e.KeyCode == Keys.Alt || e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Shift:
                if (e.KeyCode == Keys.Shift || e.KeyCode == Keys.LShiftKey || e.KeyCode == Keys.RShiftKey)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Win:
                if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                {
                    isHoldingKey = true;
                }

                break;
        }
    }

    private bool IsDelayDone(System.Windows.Point endLocation)
    {
        if (settingService.Settings.EnableHoldKey)
            return true;

        var move = Math.Abs(endLocation.X - startLocation.X) + Math.Abs(endLocation.Y - startLocation.Y);
        return move > settingService.Settings.MouseDragDelay;
    }

    private MouseButtons MouseButtonsMap(MouseButton mouseButton)
    {
        switch (mouseButton)
        {
            case MouseButton.Right:
                return MouseButtons.Right;

            case MouseButton.Middle:
                return MouseButtons.Middle;

            case MouseButton.XButton1:
                return MouseButtons.XButton1;

            case MouseButton.XButton2:
                return MouseButtons.XButton2;

            case MouseButton.Left:
            default:
                return MouseButtons.Left;
        }
    }

    private bool HoldingKeyResult()
    {
        if (settingService.Settings.EnableHoldKey)
        {
            if (isHoldingKey)
            {
                switch (settingService.Settings.HoldKeyBehaviour)
                {
                    case HoldKeyBehaviour.HoldToEnable:
                        return true;

                    case HoldKeyBehaviour.HoldToDisable:
                        SnappingCancelled?.Invoke();

                        return false;
                }
            }
            else
            {
                switch (settingService.Settings.HoldKeyBehaviour)
                {
                    case HoldKeyBehaviour.HoldToEnable:
                        return false;

                    case HoldKeyBehaviour.HoldToDisable:
                        return true;
                }
            }
        }

        return true;
    }
}