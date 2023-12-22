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
    //private static IKeyboardMouseEvents globalHook = null;

    private SimpleGlobalHook globalHook = null;
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

        globalHook = new SimpleGlobalHook();

        if (settingService.Settings.EnableMouse)
        {
            globalHook.MouseDragged += MouseMoveEvent;
            globalHook.MousePressed += MouseDownEvent;
            globalHook.MouseReleased += MouseUpEvent;
            globalHook.KeyPressed += Esc_KeyDown;
            if (settingService.Settings.EnableHoldKey)
            {
                globalHook.KeyPressed += KeyDown;
                globalHook.KeyReleased += KeyUp;
            }
        }
        _ = System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            globalHook.Run();
        });

        isWindowDetected = false;
        isListening = false;

        IsInitialized = true;
    }

    public void Dispose()
    {
        if (globalHook != null)
        {
            globalHook.MouseMoved -= MouseMoveEvent;
            globalHook.MousePressed -= MouseDownEvent;
            globalHook.MouseReleased -= MouseUpEvent;

            if (settingService.Settings.EnableHoldKey)
            {
                globalHook.KeyPressed -= KeyDown;
                globalHook.KeyReleased -= KeyUp;
            }

            //globalHook.Dispose();
        }

        IsInitialized = false;
    }

    public void Interrupt()
    {
        isListening = false;
    }

    private void MouseMoveEvent(object sender, MouseHookEventArgs e)
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

    private void MouseDownEvent(object sender, MouseHookEventArgs e)
    {
        if (e.Data.Button == MouseButtonsMap(settingService.Settings.MouseButton))
        {
            activeWindow = ActiveWindow.Empty;
            snapAreaInfo = SnapAreaInfo.Empty;
            isWindowDetected = false;
            isListening = true;

            startLocation = new System.Drawing.Point(e.Data.X, e.Data.Y);
        }
    }

    private void MouseUpEvent(object sender, MouseHookEventArgs e)
    {
        if (e.Data.Button == MouseButtonsMap(settingService.Settings.MouseButton) && isListening)
        {
            isListening = false;
            HideWindows?.Invoke();

            MoveWindow?.Invoke(new SnapAreaInfo
            {
                ActiveWindow = activeWindow,
                Rectangle = snapAreaInfo?.Rectangle
            }, e.Data.Button == SharpHook.Native.MouseButton.Button1);
        }
    }

    private void Esc_KeyDown(object? sender, KeyboardHookEventArgs e)
    {
        if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcEscape)
        {
            SnappingCancelled?.Invoke();
        }
    }

    private void KeyUp(object sender, KeyboardHookEventArgs e)
    {
        switch (settingService.Settings.HoldKey)
        {
            case HoldKey.Control:
                if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftControl || e.Data.KeyCode == SharpHook.Native.KeyCode.VcRightControl)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Alt:
                if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftAlt || e.Data.KeyCode == SharpHook.Native.KeyCode.VcRightAlt)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Shift:
                if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftShift || e.Data.KeyCode == SharpHook.Native.KeyCode.VcRightShift)
                {
                    isHoldingKey = false;
                }

                break;

            case HoldKey.Win:
                if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftMeta || e.Data.KeyCode == SharpHook.Native.KeyCode.VcRightMeta)
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

    private void KeyDown(object sender, KeyboardHookEventArgs e)
    {
        switch (settingService.Settings.HoldKey)
        {
            case HoldKey.Control:
                if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftControl || e.Data.KeyCode == SharpHook.Native.KeyCode.VcRightControl)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Alt:
                if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftAlt || e.Data.KeyCode == SharpHook.Native.KeyCode.VcRightAlt)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Shift:
                if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftShift || e.Data.KeyCode == SharpHook.Native.KeyCode.VcRightShift)
                {
                    isHoldingKey = true;
                }

                break;

            case HoldKey.Win:
                if (e.Data.KeyCode == SharpHook.Native.KeyCode.VcLeftMeta || e.Data.KeyCode == SharpHook.Native.KeyCode.VcRightMeta)
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

    private SharpHook.Native.MouseButton MouseButtonsMap(MouseButton mouseButton)
    {
        switch (mouseButton)
        {
            case MouseButton.Right:
                return SharpHook.Native.MouseButton.Button2;

            case MouseButton.Middle:
                return SharpHook.Native.MouseButton.Button3;

            case MouseButton.XButton1:
                return SharpHook.Native.MouseButton.Button4;

            case MouseButton.XButton2:
                return SharpHook.Native.MouseButton.Button5;

            case MouseButton.Left:
            default:
                return SharpHook.Native.MouseButton.Button1;
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