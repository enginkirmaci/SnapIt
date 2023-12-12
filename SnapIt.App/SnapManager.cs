using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using SnapIt.Common.Graphics;
using SnapIt.Common.Math;
using SnapIt.Controls;
using SnapIt.Services.Contracts;

namespace SnapIt.Application;

public class SnapManager : ISnapManager
{
    private readonly IWindowManager windowManager;
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private readonly IScreenManager screenManager;
    private readonly IApplicationService applicationService;
    private static List<Keys> keysDown = [];
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

    private List<ExcludedApplication> matchRulesForMouse;
    private List<ExcludedApplication> matchRulesForKeyboard;

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
        IApplicationService applicationService)
    {
        this.windowManager = windowManager;
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.screenManager = screenManager;
        this.applicationService = applicationService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        if (isTrialEnded)
            return;

        isWindowDetected = false;
        isListening = false;

        await windowManager.InitializeAsync();
        await screenManager.InitializeAsync();
        await winApiService.InitializeAsync();
        await settingService.InitializeAsync();
        await applicationService.InitializeAsync();

        if (Dev.IsActive && Dev.ShowSnapWindowOnStartup)
        {
            windowManager.Show();
        }

        globalHook = Hook.GlobalEvents();

        globalHook.KeyDown += Esc_KeyDown;

        var map = new Dictionary<Combination, Action>
        {
            { Combination.FromString(settingService.Settings.CycleLayoutsShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), CycleLayouts },
            { Combination.FromString(settingService.Settings.StartStopShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), StartStop }
        };

        Dictionary<string, Dictionary<SnapScreen, List<ApplicationGroup>>> screenApplicationGroupHotKeyMap = [];

        foreach (var snapScreen in settingService.SnapScreens)
        {
            foreach (var applicationGroup in snapScreen.ApplicationGroups)
            {
                if (!string.IsNullOrWhiteSpace(applicationGroup.ActivateHotkey))
                {
                    var applicationGroupHotkey = applicationGroup.ActivateHotkey.Replace(" ", string.Empty).Replace("Win", "LWin");

                    if (!screenApplicationGroupHotKeyMap.ContainsKey(applicationGroupHotkey))
                    {
                        screenApplicationGroupHotKeyMap.Add(applicationGroupHotkey, []);
                    }

                    if (!screenApplicationGroupHotKeyMap[applicationGroupHotkey].ContainsKey(snapScreen))
                    {
                        screenApplicationGroupHotKeyMap[applicationGroupHotkey].Add(snapScreen, []);
                    }

                    screenApplicationGroupHotKeyMap[applicationGroupHotkey][snapScreen].Add(applicationGroup);
                }
            }
        }

        if (screenApplicationGroupHotKeyMap.Count > 0)
        {
            foreach (var screenApplicationGroupHotkey in screenApplicationGroupHotKeyMap)
            {
                map.Add(Combination.FromString(screenApplicationGroupHotkey.Key), () =>
                {
                    foreach (var screenApplicationGroup in screenApplicationGroupHotkey.Value)
                    {
                        foreach (var applicationGroup in screenApplicationGroup.Value)
                        {
                            StartApplications(screenApplicationGroup.Key, applicationGroup);
                        }
                    }
                });
            }
        }

        if (settingService.Settings.EnableKeyboard)
        {
            map.Add(Combination.FromString(settingService.Settings.MoveLeftShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), () => MoveActiveWindowByKeyboard(MoveDirection.Left));
            map.Add(Combination.FromString(settingService.Settings.MoveRightShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), () => MoveActiveWindowByKeyboard(MoveDirection.Right));
            map.Add(Combination.FromString(settingService.Settings.MoveUpShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), () => MoveActiveWindowByKeyboard(MoveDirection.Up));
            map.Add(Combination.FromString(settingService.Settings.MoveDownShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), () => MoveActiveWindowByKeyboard(MoveDirection.Down));

            if ((settingService.Settings.MoveLeftShortcut +
                settingService.Settings.MoveRightShortcut +
                settingService.Settings.MoveUpShortcut +
                settingService.Settings.MoveDownShortcut).Contains("Win"))
            {
                globalHook.KeyDown += HookManager_KeyDown;
                globalHook.KeyUp += HookManager_KeyUp;
            }
        }

        globalHook.OnCombination(map);

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

        if (settingService.ExcludedApplicationSettings?.Applications != null)
        {
            matchRulesForMouse = settingService.ExcludedApplicationSettings.Applications.Where(i => i.Mouse).ToList();
            matchRulesForKeyboard = settingService.ExcludedApplicationSettings.Applications.Where(i => i.Keyboard).ToList();
        }

        IsRunning = true;
        StatusChanged?.Invoke(true);
        ScreenLayoutLoaded?.Invoke(settingService.SnapScreens, settingService.Layouts);

        IsInitialized = true;
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
        if (DisableIfFullScreen())
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

    private bool DisableIfFullScreen()
    {
        activeWindow = winApiService.GetActiveWindow();

        if (activeWindow != ActiveWindow.Empty && !string.IsNullOrWhiteSpace(activeWindow.Title) && !activeWindow.Title.Equals("Program Manager") && settingService.Settings.DisableForFullscreen && winApiService.IsFullscreen(activeWindow))
        {
            return true;
        }

        return false;
    }

    private async Task StartApplication(ApplicationItem application, Rectangle rectangle)
    {
        var openedWindow = await applicationService.StartApplication(application, rectangle);

        if (openedWindow != null)
        {
            MoveWindow(openedWindow, rectangle, false);
        }
    }

    private void StartStop()
    {
        if (DisableIfFullScreen())
        {
            return;
        }

        if (IsRunning)
        {
            Release();
        }
        else
        {
            _ = InitializeAsync();
        }
    }

    public void Release()
    {
        windowManager.Release();

        if (globalHook != null)
        {
            globalHook.KeyDown -= Esc_KeyDown;

            globalHook.MouseMove -= MouseMoveEvent;
            globalHook.MouseDown -= MouseDownEvent;
            globalHook.MouseUp -= MouseUpEvent;

            if (settingService.Settings.EnableHoldKey)
            {
                globalHook.KeyDown -= GlobalHook_KeyDown;
                globalHook.KeyUp -= GlobalHook_KeyUp;
            }

            if (settingService.Settings.EnableKeyboard)
            {
                if ((settingService.Settings.MoveLeftShortcut +
                    settingService.Settings.MoveRightShortcut +
                    settingService.Settings.MoveUpShortcut +
                    settingService.Settings.MoveDownShortcut).Contains("Win"))
                {
                    globalHook.KeyDown -= HookManager_KeyDown;
                    globalHook.KeyUp -= HookManager_KeyUp;
                }
            }

            globalHook.Dispose();
        }

        globalHook = Hook.GlobalEvents();

        var map = new Dictionary<Combination, Action>
        {
            { Combination.FromString(settingService.Settings.StartStopShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), StartStop }
        };

        globalHook.OnCombination(map);

        IsRunning = false;
        StatusChanged?.Invoke(false);
        IsInitialized = false;
    }

    private void CycleLayouts()
    {
        if (DisableIfFullScreen())
        {
            return;
        }

        var snapScreen = settingService.LatestActiveScreen;
        var layoutIndex = settingService.Layouts.IndexOf(snapScreen.Layout);
        var nextLayout = settingService.Layouts.ElementAt((layoutIndex + 1) % settingService.Layouts.Count);

        settingService.LinkScreenLayout(snapScreen, nextLayout);
        Release();
        _ = InitializeAsync();

        LayoutChanged?.Invoke(snapScreen, nextLayout);
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

    private static void HookManager_KeyDown(object sender, KeyEventArgs e)
    {
        //Used for overriding the Windows default hotkeys
        if (keysDown.Contains(e.KeyCode) == false)
        {
            keysDown.Add(e.KeyCode);
        }

        if (e.KeyCode == Keys.Right && WIN())
        {
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Left && WIN())
        {
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Up && WIN())
        {
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Down && WIN())
        {
            e.Handled = true;
        }
    }

    private static void HookManager_KeyUp(object sender, KeyEventArgs e)
    {
        //Used for overriding the Windows default hotkeys
        while (keysDown.Contains(e.KeyCode))
        {
            keysDown.Remove(e.KeyCode);
        }
    }

    private static bool WIN()
    {
        if (keysDown.Contains(Keys.LWin) ||
            keysDown.Contains(Keys.RWin))
        {
            return true;
        }
        else
        {
            return false;
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

    private void MoveActiveWindowByKeyboard(MoveDirection direction)
    {
        activeWindow = winApiService.GetActiveWindow();

        if (activeWindow != ActiveWindow.Empty)
        {
            if (settingService.Settings.DisableForFullscreen && winApiService.IsFullscreen(activeWindow) ||
               settingService.Settings.DisableForModal && !winApiService.IsAllowedWindowStyle(activeWindow) ||
                IsExcludedApplication(activeWindow.Title, true))
            {
                return;
            }

            var boundries = windowManager.SnapAreaBoundries();
            if (boundries != null)
            {
                winApiService.GetWindowMargin(activeWindow, out Rectangle rectmargin);
                var activeBoundry = boundries.FirstOrDefault(i => i.Contains(rectmargin));
                activeWindow.Dpi = DpiHelper.GetDpiFromPoint(activeBoundry.Left, activeBoundry.Right);

                var newSnapArea = FindClosest.GetClosestRectangle(boundries, activeBoundry, direction);

                MoveActiveWindow(newSnapArea ?? activeBoundry, false);

                Telemetry.TrackEvent("MoveActiveWindow - Keyboard");
            }
        }
    }

    private void StopSnapping()
    {
        windowManager.Hide();
        isListening = false;
    }

    public static bool WildcardMatch(string pattern, string input, bool caseSensitive = false)
    {
        pattern = pattern.Replace(".", @"\.");
        pattern = pattern.Replace("?", ".");
        pattern = pattern.Replace("*", ".*?");
        pattern = pattern.Replace(@"\", @"\\");
        pattern = pattern.Replace(" ", @"\s");
        return new Regex(pattern, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase).IsMatch(input);
    }

    private bool IsExcludedApplication(string Title, bool isKeyboard)
    {
        if (settingService.ExcludedApplicationSettings?.Applications != null)
        {
            var matchRules = isKeyboard ? matchRulesForKeyboard : matchRulesForMouse;

            if (matchRules != null)
            {
                var isMatched = false;
                foreach (var rule in matchRules)
                {
                    if (string.IsNullOrWhiteSpace(rule.Keyword))
                    {
                        continue;
                    }

                    switch (rule.MatchRule)
                    {
                        case MatchRule.Contains:
                            isMatched = Title.Contains(rule.Keyword, StringComparison.OrdinalIgnoreCase);
                            break;

                        case MatchRule.Exact:
                            isMatched = Title == rule.Keyword;
                            break;

                        case MatchRule.Wildcard:
                            isMatched = WildcardMatch(rule.Keyword, Title, false);
                            break;
                    }

                    if (isMatched)
                    {
                        break;
                    }
                }

                return isMatched;
            }
        }

        return false;
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

                if (activeWindow?.Title != null && IsExcludedApplication(activeWindow.Title, false))
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

                if (!withMargin.Equals(default(Rectangle)))
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

                        if (!rectangle.Dpi.Equals(currentWindow.Dpi))
                        {
                            winApiService.MoveWindow(currentWindow, rectangle);
                        }
                    }).Start();
                }
                else
                {
                    winApiService.MoveWindow(currentWindow, rectangle);

                    if (!rectangle.Dpi.Equals(currentWindow.Dpi))
                    {
                        winApiService.MoveWindow(currentWindow, rectangle);
                    }
                }

                Telemetry.TrackEvent("MoveActiveWindow - Mouse");
            }
        }
    }
}