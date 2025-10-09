using System.Windows.Forms;
using SharpHook;
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
    private readonly IGlobalHookService globalHookService;
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
        IWindowsService windowsService,
        IGlobalHookService globalHookService)
    {
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.windowsService = windowsService;
        this.globalHookService = globalHookService;
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
        await globalHookService.InitializeAsync();

        if (globalHookService.Hook != null && settingService.Settings.EnableMouse)
        {
            globalHookService.Hook.MouseDragged += MouseMoveEvent;
            globalHookService.Hook.MousePressed += MouseDownEvent;
            globalHookService.Hook.MouseReleased += MouseUpEvent;
            globalHookService.Hook.KeyPressed += Esc_KeyDown;

            if (settingService.Settings.EnableHoldKey)
            {
                globalHookService.Hook.KeyPressed += KeyDown;
                globalHookService.Hook.KeyReleased += KeyUp;
            }
        }

        isWindowDetected = false;
        isListening = false;

        IsInitialized = true;
    }

    public void Dispose()
    {
        if (globalHookService.Hook != null)
        {
            globalHookService.Hook.MouseDragged -= MouseMoveEvent;
            globalHookService.Hook.MousePressed -= MouseDownEvent;
            globalHookService.Hook.MouseReleased -= MouseUpEvent;

            if (settingService.Settings.EnableHoldKey)
            {
                globalHookService.Hook.KeyPressed -= KeyDown;
                globalHookService.Hook.KeyReleased -= KeyUp;
            }
        }

        IsInitialized = false;
    }

    public void Interrupt()
    {
        isListening = false;
    }

    private void MouseMoveEvent(object? sender, MouseHookEventArgs e)
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

    private void MouseDownEvent(object? sender, MouseHookEventArgs e)
    {
        if (e.Data.Button == MouseButtonsMap(settingService.Settings.MouseButton))
        {
            globalHookService.Hook.MouseDragged += MouseMoveEvent;

            activeWindow = ActiveWindow.Empty;
            snapAreaInfo = SnapAreaInfo.Empty;
            isWindowDetected = false;
            isListening = true;

            startLocation = new System.Drawing.Point(e.Data.X, e.Data.Y);
        }
    }

    private void MouseUpEvent(object? sender, MouseHookEventArgs e)
    {
        if (e.Data.Button == MouseButtonsMap(settingService.Settings.MouseButton) && isListening)
        {
            globalHookService.Hook.MouseDragged -= MouseMoveEvent;

            isListening = false;
            HideWindows?.Invoke();

            MoveWindow?.Invoke(new SnapAreaInfo
            {
                ActiveWindow = activeWindow,
                Rectangle = snapAreaInfo?.Rectangle
            }, e.Data.Button == SharpHook.Data.MouseButton.Button1);
        }
    }

    private void Esc_KeyDown(object? sender, KeyboardHookEventArgs e)
    {
        if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcEscape)
        {
            SnappingCancelled?.Invoke();
        }
    }

    private void KeyUp(object? sender, KeyboardHookEventArgs e)
    {
        switch (settingService.Settings.HoldKey)
        {
            case HoldKey.Control:
                if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcLeftControl || e.Data.KeyCode == SharpHook.Data.KeyCode.VcRightControl)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Alt:
                if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcLeftAlt || e.Data.KeyCode == SharpHook.Data.KeyCode.VcRightAlt)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Shift:
                if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcLeftShift || e.Data.KeyCode == SharpHook.Data.KeyCode.VcRightShift)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Win:
                if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcLeftMeta || e.Data.KeyCode == SharpHook.Data.KeyCode.VcRightMeta)
                {
                    isHoldingKey = false;

                    if (holdKeyUsed)
                    {
                        e.SuppressEvent = true;
                    }
                }

                break;
        }

        if (holdKeyUsed)
        {
            holdKeyUsed = false;
        }
    }

    private void KeyDown(object? sender, KeyboardHookEventArgs e)
    {
        switch (settingService.Settings.HoldKey)
        {
            case HoldKey.Control:
                if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcLeftControl || e.Data.KeyCode == SharpHook.Data.KeyCode.VcRightControl)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Alt:
                if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcLeftAlt || e.Data.KeyCode == SharpHook.Data.KeyCode.VcRightAlt)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Shift:
                if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcLeftShift || e.Data.KeyCode == SharpHook.Data.KeyCode.VcRightShift)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Win:
                if (e.Data.KeyCode == SharpHook.Data.KeyCode.VcLeftMeta || e.Data.KeyCode == SharpHook.Data.KeyCode.VcRightMeta)
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

    private SharpHook.Data.MouseButton MouseButtonsMap(MouseButton mouseButton)
    {
        switch (mouseButton)
        {
            case MouseButton.Right:
                return SharpHook.Data.MouseButton.Button2;

            case MouseButton.Middle:
                return SharpHook.Data.MouseButton.Button3;

            case MouseButton.XButton1:
                return SharpHook.Data.MouseButton.Button4;

            case MouseButton.XButton2:
                return SharpHook.Data.MouseButton.Button5;

            case MouseButton.Left:
            default:
                return SharpHook.Data.MouseButton.Button1;
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