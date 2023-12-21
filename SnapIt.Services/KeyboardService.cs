using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Events;
using SnapIt.Common.Extensions;
using SnapIt.Common.Graphics;
using SnapIt.Common.Math;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class KeyboardService : IKeyboardService
{
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private readonly IWindowsService windowsService;
    private static IKeyboardMouseEvents globalHook = null;
    private List<Keys> keysDown = [];

    public bool IsInitialized { get; private set; }

    public event SnappingCancelEvent SnappingCancelled;

    public event SnapStartStopEvent SnapStartStop;

    public event MoveWindowEvent MoveWindow;

    public event GetSnapAreaBoundriesEvent GetSnapAreaBoundries;

    public event ChangeLayoutEvent ChangeLayout;

    public KeyboardService(
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
        globalHook.KeyDown += Esc_KeyDown;

        var map = new Dictionary<Combination, Action>
        {
            { Combination.FromString(settingService.Settings.CycleLayoutsShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), CycleLayouts },
            { Combination.FromString(settingService.Settings.StartStopShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), StartStopSnapping }
        };

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

        IsInitialized = true;
    }

    public void SetSnappingStopped()
    {
        if (settingService.Settings != null)
        {
            var map = new Dictionary<Combination, Action>
            {
                { Combination.FromString(settingService.Settings.StartStopShortcut.Replace(" ", string.Empty).Replace("Win", "LWin")), StartStopSnapping }
            };

            globalHook?.OnCombination(map);
        }
    }

    public void Dispose()
    {
        if (globalHook != null)
        {
            globalHook.KeyDown -= Esc_KeyDown;

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
        }

        IsInitialized = false;
    }

    private void StartStopSnapping()
    {
        if (windowsService.DisableIfFullScreen())
        {
            return;
        }

        SnapStartStop?.Invoke();
    }

    private void CycleLayouts()
    {
        if (windowsService.DisableIfFullScreen())
        {
            return;
        }

        var snapScreen = settingService.LatestActiveScreen;
        var layoutIndex = settingService.Layouts.IndexOf(snapScreen.Layout);
        var nextLayout = settingService.Layouts.ElementAt((layoutIndex + 1) % settingService.Layouts.Count);

        settingService.LinkScreenLayout(snapScreen, nextLayout);

        ChangeLayout?.Invoke(snapScreen, nextLayout);
    }

    private void Esc_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            SnappingCancelled?.Invoke();
        }
    }

    private void MoveActiveWindowByKeyboard(MoveDirection direction)
    {
        var activeWindow = winApiService.GetActiveWindow();

        if (activeWindow != ActiveWindow.Empty)
        {
            if (settingService.Settings.DisableForFullscreen && winApiService.IsFullscreen(activeWindow) ||
               settingService.Settings.DisableForModal && !winApiService.IsAllowedWindowStyle(activeWindow) ||
               windowsService.IsExcludedApplication(activeWindow.Title, true))
            {
                return;
            }

            var boundries = GetSnapAreaBoundries?.Invoke();

            if (boundries != null)
            {
                winApiService.GetWindowMargin(activeWindow, out Rectangle rectmargin);
                var activeBoundry = boundries.FirstOrDefault(i => i.Contains(rectmargin));
                activeWindow.Dpi = DpiHelper.GetDpiFromPoint(activeBoundry.Left, activeBoundry.Right);

                var newSnapArea = FindClosest.GetClosestRectangle(boundries, activeBoundry, direction);

                MoveWindow?.Invoke(new SnapAreaInfo
                {
                    ActiveWindow = activeWindow,
                    Rectangle = newSnapArea ?? activeBoundry
                }, false);

                Telemetry.TrackEvent("MoveActiveWindow - Keyboard");
            }
        }
    }

    private void HookManager_KeyDown(object sender, KeyEventArgs e)
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

    private void HookManager_KeyUp(object sender, KeyEventArgs e)
    {
        //Used for overriding the Windows default hotkeys
        while (keysDown.Contains(e.KeyCode))
        {
            keysDown.Remove(e.KeyCode);
        }
    }

    private bool WIN()
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
}