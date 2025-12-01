using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;
using SnapIt.Controls;
using SnapIt.Services.Contracts;

namespace SnapIt.Application;

public class SnapManager : ISnapManager
{
    private readonly IWindowManager windowManager;
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private readonly IScreenManager screenManager;
    private readonly IMouseService mouseService;
    private readonly IKeyboardService keyboardService;
    private readonly IWindowsService windowsService;
    private readonly IWindowEventService windowEventService;

    private SnapLoadingWindow loadingWindow;
    private bool isTrialEnded = false;

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
        IMouseService mouseService,
        IKeyboardService keyboardService,
        IWindowsService windowsService,
        IWindowEventService windowEventService)
    {
        this.windowManager = windowManager;
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.screenManager = screenManager;
        this.mouseService = mouseService;
        this.keyboardService = keyboardService;
        this.windowsService = windowsService;
        this.windowEventService = windowEventService;

        keyboardService.SnapStartStop += KeyboardService_SnapStartStop;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        if (isTrialEnded)
            return;

        //globalHook = Hook.GlobalEvents();

        await windowManager.InitializeAsync();
        await screenManager.InitializeAsync();
        await winApiService.InitializeAsync();
        await settingService.InitializeAsync();
        await keyboardService.InitializeAsync();
        await mouseService.InitializeAsync();
        await windowsService.InitializeAsync();
        await windowEventService.InitializeAsync();
        windowEventService.StartMonitoring();

        if (Dev.IsActive && Dev.ShowSnapWindowOnStartup)
        {
            windowManager.Show();
        }

        screenManager.SetSnapManager(this);

        mouseService.MoveWindow += MoveWindow;
        mouseService.SnappingCancelled += SnappingCancelled;

        keyboardService.MoveWindow += MoveWindow;
        keyboardService.SnappingCancelled += SnappingCancelled;
        keyboardService.ChangeLayout += KeyboardService_ChangeLayout;

        IsRunning = true;
        StatusChanged?.Invoke(true);
        ScreenLayoutLoaded?.Invoke(settingService.SnapScreens, settingService.Layouts);

        IsInitialized = true;
    }

    public void StartStop()
    {
        if (IsRunning)
        {
            Dispose();
        }
        else
        {
            _ = InitializeAsync();
        }
    }

    private void MoveWindow(SnapAreaInfo snapAreaInfo, bool isLeftClick)
    {
        MoveWindow(snapAreaInfo.ActiveWindow, snapAreaInfo.Rectangle, isLeftClick);
    }

    private void SnappingCancelled()
    {
        windowManager.Hide();
        mouseService.Interrupt();
    }

    private void KeyboardService_SnapStartStop()
    {
        StartStop();
    }

    private void KeyboardService_ChangeLayout(SnapScreen snapScreen, Layout layout)
    {
        Dispose();
        _ = InitializeAsync();

        LayoutChanged?.Invoke(snapScreen, layout);
    }

    public void SetIsTrialEnded(bool isEnded)
    {
        if (isEnded)
        {
            isTrialEnded = true;
            Dispose();
        }
        else
        {
            isTrialEnded = false;
        }
    }

    //public async Task StartApplications(SnapScreen snapScreen, ApplicationGroup applicationGroup)
    //{
    //    if (windowsService.DisableIfFullScreen())
    //    {
    //        return;
    //    }

    //    await applicationService.InitializeAsync();

    //    var areaRectangles = windowManager.GetSnapAreaRectangles(snapScreen);

    //    foreach (var area in applicationGroup.ApplicationAreas)
    //    {
    //        if (area.Applications != null)
    //        {
    //            foreach (var application in area.Applications)
    //            {
    //                System.Windows.Application.Current.Dispatcher.Invoke(() =>
    //                {
    //                    if (loadingWindow == null)
    //                    {
    //                        var primaryScreen = settingService.SnapScreens.FirstOrDefault(i => i.IsPrimary);
    //                        if (primaryScreen == null)
    //                        {
    //                            primaryScreen = settingService.SnapScreens.First();
    //                        }

    //                        loadingWindow = new SnapLoadingWindow(winApiService, primaryScreen);
    //                    }

    //                    loadingWindow.SetLoadingMessage(
    //                            !string.IsNullOrWhiteSpace(application?.Title) ?
    //                            application?.Title : application?.Path);
    //                });

    //                if (areaRectangles != null && application != null && areaRectangles.ContainsKey(application.AreaNumber))
    //                {
    //                    await StartApplication(application, areaRectangles[application.AreaNumber]);
    //                }
    //            }
    //        }
    //    }

    //    loadingWindow.Hide();

    //    applicationService.Clear();
    //}

    //private async Task StartApplication(ApplicationItem application, Rectangle rectangle)
    //{
    //    var openedWindow = await applicationService.StartApplication(application, rectangle);

    //    if (openedWindow != null)
    //    {
    //        MoveWindow(openedWindow, rectangle, false);
    //    }
    //}

    public void Dispose()
    {
        windowManager.Dispose();

        mouseService.MoveWindow -= MoveWindow;
        mouseService.SnappingCancelled -= SnappingCancelled;
        mouseService.Dispose();

        keyboardService.MoveWindow -= MoveWindow;
        keyboardService.SnappingCancelled -= SnappingCancelled;
        //keyboardService.SnapStartStop -= KeyboardService_SnapStartStop;
        windowEventService.StopMonitoring();
        keyboardService.ChangeLayout -= KeyboardService_ChangeLayout;
        keyboardService.Dispose();

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
            Dispose();
            _ = InitializeAsync();
        }

        ScreenChanged?.Invoke(settingService.SnapScreens);
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
