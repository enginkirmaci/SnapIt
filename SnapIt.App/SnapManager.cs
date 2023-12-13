using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using SnapIt.Common.Graphics;
using SnapIt.Controls;
using SnapIt.Services;
using SnapIt.Services.Contracts;

namespace SnapIt.Application;

public class SnapManager : ISnapManager
{
    private readonly IWindowManager windowManager;
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private readonly IScreenManager screenManager;
    private readonly IApplicationService applicationService;
    private readonly IKeyboardService keyboardService;
    private readonly IWindowsService windowsService;
    private static IKeyboardMouseEvents globalHook;

    private ActiveWindow activeWindow;
    private SnapAreaInfo snapAreaInfo;

    private SnapLoadingWindow loadingWindow;
    private bool isTrialEnded = false;

    private bool isWindowDetected = false;
    private bool isListening = false;
    private bool isHoldingKey = false;
    private bool holdKeyUsed = false;

    private System.Drawing.Point startLocation;

    public bool IsInitialized { get; private set; }
    public bool IsRunning { get; set; }

    public event GetStatus StatusChanged;

    public event ScreenChangedEvent ScreenChanged;

    public event LayoutChangedEvent LayoutChanged;

    public event ScreenLayoutLoadedEvent ScreenLayoutLoaded;

    public SnapManager(
        IWindowManager windowManager,
        ISettingService settingService,
        IWinApiService winApiService,
        IScreenManager screenManager,
        IApplicationService applicationService,
        IKeyboardService keyboardService,
        IWindowsService windowsService)
    {
        this.windowManager = windowManager;
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.screenManager = screenManager;
        this.applicationService = applicationService;
        this.keyboardService = keyboardService;
        this.windowsService = windowsService;
    }

    public async Task InitializeAsync()
    {
        Dev.Log();

        if (IsInitialized)
        {
            return;
        }

        if (isTrialEnded)
            return;

        isWindowDetected = false;
        isListening = false;

        globalHook = Hook.GlobalEvents();

        await windowManager.InitializeAsync();
        await screenManager.InitializeAsync();
        await winApiService.InitializeAsync();
        await settingService.InitializeAsync();
        await applicationService.InitializeAsync();
        await keyboardService.InitializeAsync();
        await windowsService.InitializeAsync();

        if (Dev.IsActive && Dev.ShowSnapWindowOnStartup)
        {
            windowManager.Show();
        }

        keyboardService.SnappingCancelled += KeyboardService_SnappingCancelled;
        keyboardService.SnapStartStop += KeyboardService_SnapStartStop;
        keyboardService.MoveWindow += KeyboardService_MoveWindow;
        keyboardService.ChangeLayout += KeyboardService_ChangeLayout;

        //Dictionary<string, Dictionary<SnapScreen, List<ApplicationGroup>>> screenApplicationGroupHotKeyMap = [];

        //foreach (var snapScreen in settingService.SnapScreens)
        //{
        //    foreach (var applicationGroup in snapScreen.ApplicationGroups)
        //    {
        //        if (!string.IsNullOrWhiteSpace(applicationGroup.ActivateHotkey))
        //        {
        //            var applicationGroupHotkey = applicationGroup.ActivateHotkey.Replace(" ", string.Empty).Replace("Win", "LWin");

        //            if (!screenApplicationGroupHotKeyMap.ContainsKey(applicationGroupHotkey))
        //            {
        //                screenApplicationGroupHotKeyMap.Add(applicationGroupHotkey, []);
        //            }

        //            if (!screenApplicationGroupHotKeyMap[applicationGroupHotkey].ContainsKey(snapScreen))
        //            {
        //                screenApplicationGroupHotKeyMap[applicationGroupHotkey].Add(snapScreen, []);
        //            }

        //            screenApplicationGroupHotKeyMap[applicationGroupHotkey][snapScreen].Add(applicationGroup);
        //        }
        //    }
        //}

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

        IsRunning = true;
        StatusChanged?.Invoke(true);
        ScreenLayoutLoaded?.Invoke(settingService.SnapScreens, settingService.Layouts);

        IsInitialized = true;
    }

    public void StartStop()
    {
        if (IsRunning)
        {
            Release();
        }
        else
        {
            _ = InitializeAsync();
        }
    }

    private void KeyboardService_ChangeLayout(SnapScreen snapScreen, Layout layout)
    {
        Release();
        _ = InitializeAsync();

        LayoutChanged?.Invoke(snapScreen, layout);
    }

    private void KeyboardService_MoveWindow(SnapAreaInfo snapAreaInfo, bool isLeftClick)
    {
        MoveWindow(snapAreaInfo.ActiveWindow, snapAreaInfo.Rectangle, isLeftClick);
    }

    private void KeyboardService_SnapStartStop()
    {
        StartStop();
    }

    private void KeyboardService_SnappingCancelled()
    {
        StopSnapping();
    }

    public void SetIsTrialEnded(bool isEnded)
    {
        if (isEnded)
        {
            isTrialEnded = true;
            Release();
        }
        else
        {
            isTrialEnded = false;
        }
    }

    public async Task StartApplications(SnapScreen snapScreen, ApplicationGroup applicationGroup)
    {
        if (windowsService.DisableIfFullScreen())
        {
            return;
        }

        await applicationService.InitializeAsync();

        var areaRectangles = windowManager.GetSnapAreaRectangles(snapScreen);

        foreach (var area in applicationGroup.ApplicationAreas)
        {
            if (area.Applications != null)
            {
                foreach (var application in area.Applications)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (loadingWindow == null)
                        {
                            var primaryScreen = settingService.SnapScreens.FirstOrDefault(i => i.IsPrimary);
                            if (primaryScreen == null)
                            {
                                primaryScreen = settingService.SnapScreens.First();
                            }

                            loadingWindow = new SnapLoadingWindow(winApiService, primaryScreen);
                        }

                        loadingWindow.SetLoadingMessage(
                                !string.IsNullOrWhiteSpace(application?.Title) ?
                                application?.Title : application?.Path);
                    });

                    if (areaRectangles != null && application != null && areaRectangles.ContainsKey(application.AreaNumber))
                    {
                        await StartApplication(application, areaRectangles[application.AreaNumber]);
                    }
                }
            }
        }

        loadingWindow.Hide();

        applicationService.Clear();
    }

    private async Task StartApplication(ApplicationItem application, Rectangle rectangle)
    {
        var openedWindow = await applicationService.StartApplication(application, rectangle);

        if (openedWindow != null)
        {
            MoveWindow(openedWindow, rectangle, false);
        }
    }

    public void Release()
    {
        windowManager.Release();

        keyboardService.SnappingCancelled -= KeyboardService_SnappingCancelled;
        keyboardService.SnapStartStop -= KeyboardService_SnapStartStop;
        keyboardService.MoveWindow -= KeyboardService_MoveWindow;
        keyboardService.ChangeLayout -= KeyboardService_ChangeLayout;
        keyboardService.Dispose();

        if (globalHook != null)
        {
            //globalHook.KeyDown -= Esc_KeyDown;

            globalHook.MouseMove -= MouseMoveEvent;
            globalHook.MouseDown -= MouseDownEvent;
            globalHook.MouseUp -= MouseUpEvent;

            if (settingService.Settings.EnableHoldKey)
            {
                globalHook.KeyDown -= GlobalHook_KeyDown;
                globalHook.KeyUp -= GlobalHook_KeyUp;
            }

            globalHook.Dispose();
        }
        if (globalHook != null)
        {
            globalHook.Dispose();
        }

        globalHook = Hook.GlobalEvents();

        keyboardService.SetSnappingStopped();

        IsRunning = false;
        StatusChanged?.Invoke(false);
        IsInitialized = false;
    }

    public void ScreenChangedEvent()
    {
        settingService.ReInitialize();

        if (IsRunning)
        {
            Release();
            _ = InitializeAsync();
        }

        ScreenChanged?.Invoke(settingService.SnapScreens);
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
                        StopSnapping();
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

    private void Esc_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            StopSnapping();
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

    private void StopSnapping()
    {
        windowManager.Hide();
        isListening = false;
    }

    private bool IsDelayDone(System.Windows.Point endLocation)
    {
        if (settingService.Settings.EnableHoldKey)
            return true;

        var move = Math.Abs(endLocation.X - startLocation.X) + Math.Abs(endLocation.Y - startLocation.Y);
        return move > settingService.Settings.MouseDragDelay;
    }

    private void MouseMoveEvent(object sender, MouseEventArgs e)
    {
        var p = WpfScreenHelper.MouseHelper.MousePosition;

        if (isListening && HoldingKeyResult() && IsDelayDone(p))
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
            else if (!windowManager.IsVisible)
            {
                windowManager.Show();
            }
            else
            {
                snapAreaInfo = windowManager.SelectElementWithPoint((int)p.X, (int)p.Y);

                if (snapAreaInfo?.Screen != null)
                {
                    settingService.LatestActiveScreen = snapAreaInfo.Screen;
                }
            }
        }
    }

    public MouseButtons MouseButtonsMap(MouseButton mouseButton)
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
            windowManager.Hide();

            MoveActiveWindow(snapAreaInfo.Rectangle, e.Button == MouseButtons.Left);
        }
    }

    private void MoveActiveWindow(Rectangle rectangle, bool isLeftClick)
    {
        MoveWindow(activeWindow, rectangle, isLeftClick);
    }

    private void MoveWindow(ActiveWindow currentWindow, Rectangle rectangle, bool isLeftClick)
    {
        if (currentWindow != ActiveWindow.Empty)
        {
            if (rectangle != null && !rectangle.Equals(Rectangle.Empty))
            {
                winApiService.GetWindowMargin(currentWindow, out Rectangle withMargin);

                if (!withMargin.Equals(Rectangle.Empty))
                {
                    var marginHorizontal = (currentWindow.Boundry.Width - withMargin.Width) / 2;
                    var systemMargin = new Rectangle
                    {
                        Left = marginHorizontal,
                        Right = marginHorizontal,
                        Top = 0,
                        Bottom = currentWindow.Boundry.Height - withMargin.Height
                    };

                    rectangle.Left -= systemMargin.Left;
                    rectangle.Top -= systemMargin.Top;
                    rectangle.Right += systemMargin.Right;
                    rectangle.Bottom += systemMargin.Bottom;
                }

                if (isLeftClick)
                {
                    new Thread(() =>
                    {
                        Thread.Sleep(100);

                        winApiService.MoveWindow(currentWindow, rectangle);

                        if (!rectangle.Dpi.Equals(currentWindow?.Dpi))
                        {
                            winApiService.MoveWindow(currentWindow, rectangle);
                        }
                    }).Start();
                }
                else
                {
                    winApiService.MoveWindow(currentWindow, rectangle);

                    if (!rectangle.Dpi.Equals(currentWindow?.Dpi))
                    {
                        winApiService.MoveWindow(currentWindow, rectangle);
                    }
                }

                Telemetry.TrackEvent("MoveActiveWindow - Mouse");
            }
        }
    }
}