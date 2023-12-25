using GlobalHotKey;
using SharpHook;
using SharpHook.Native;
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
    private readonly IHotkeyService hotkeyService;
    private readonly IGlobalHookService globalHookService;
    private List<KeyCode> keysDown = [];

    public bool IsInitialized { get; private set; }

    public event SnappingCancelEvent SnappingCancelled;

    public event SnapStartStopEvent SnapStartStop;

    public event MoveWindowEvent MoveWindow;

    public event GetSnapAreaBoundriesEvent GetSnapAreaBoundries;

    public event ChangeLayoutEvent ChangeLayout;

    public KeyboardService(
        ISettingService settingService,
        IWinApiService winApiService,
        IWindowsService windowsService,
        IHotkeyService hotkeyService,
        IGlobalHookService globalHookService)
    {
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.windowsService = windowsService;
        this.hotkeyService = hotkeyService;
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

        if (settingService.Settings.EnableKeyboard)
        {
            hotkeyService.KeyPressed -= HotkeyService_KeyPressed;
            hotkeyService.KeyPressed += HotkeyService_KeyPressed;

            await hotkeyService.InitializeAsync();

            if (globalHookService.Hook != null)
            {
                globalHookService.Hook.KeyPressed += Esc_KeyDown;

                if ((settingService.Settings.MoveLeftShortcut +
                    settingService.Settings.MoveRightShortcut +
                    settingService.Settings.MoveUpShortcut +
                    settingService.Settings.MoveDownShortcut).Contains("Win"))
                {
                    globalHookService.Hook.KeyPressed += HookManager_KeyDown;
                    globalHookService.Hook.KeyReleased += HookManager_KeyUp;
                }
            }
        }

        IsInitialized = true;
    }

    public void Dispose()
    {
        if (globalHookService.Hook != null)
        {
            globalHookService.Hook.KeyPressed -= Esc_KeyDown;

            if (settingService.Settings.EnableKeyboard)
            {
                hotkeyService.KeyPressed -= HotkeyService_KeyPressed;
                hotkeyService.Dispose();

                if ((settingService.Settings.MoveLeftShortcut +
                    settingService.Settings.MoveRightShortcut +
                    settingService.Settings.MoveUpShortcut +
                    settingService.Settings.MoveDownShortcut).Contains("Win"))
                {
                    globalHookService.Hook.KeyPressed -= HookManager_KeyDown;
                    globalHookService.Hook.KeyReleased -= HookManager_KeyUp;
                }
            }
        }

        IsInitialized = false;
    }

    private void HotkeyService_KeyPressed(object? sender, KeyPressedEventArgs e)
    {
        if (e.HotKey.Equals(hotkeyService.CycleLayoutsHotKey))
        {
            CycleLayouts();
        }
        if (e.HotKey.Equals(hotkeyService.StartStopHotKey))
        {
            StartStopSnapping();
        }
        if (e.HotKey.Equals(hotkeyService.MoveLeftHotKey))
        {
            MoveActiveWindowByKeyboard(MoveDirection.Left);
        }
        if (e.HotKey.Equals(hotkeyService.MoveRightHotKey))
        {
            MoveActiveWindowByKeyboard(MoveDirection.Right);
        }
        if (e.HotKey.Equals(hotkeyService.MoveUpHotKey))
        {
            MoveActiveWindowByKeyboard(MoveDirection.Up);
        }
        if (e.HotKey.Equals(hotkeyService.MoveDownHotKey))
        {
            MoveActiveWindowByKeyboard(MoveDirection.Down);
        }
    }

    public void SetSnappingStopped()
    {
        if (settingService.Settings.EnableKeyboard)
        {
            hotkeyService.KeyPressed -= HotkeyService_KeyPressed;
            hotkeyService.KeyPressed += HotkeyService_KeyPressed;

            hotkeyService.RegisterStartStopHotkey();
        }
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

    private void Esc_KeyDown(object? sender, KeyboardHookEventArgs e)
    {
        if (e.Data.KeyCode == KeyCode.VcEscape)
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

    private void HookManager_KeyDown(object? sender, KeyboardHookEventArgs e)
    {
        //Used for overriding the Windows default hotkeys
        if (keysDown.Contains(e.Data.KeyCode) == false)
        {
            keysDown.Add(e.Data.KeyCode);
        }

        if (e.Data.KeyCode == KeyCode.VcRight && WIN())
        {
            MoveActiveWindowByKeyboard(MoveDirection.Right);
            e.SuppressEvent = true;
        }
        else if (e.Data.KeyCode == KeyCode.VcLeft && WIN())
        {
            MoveActiveWindowByKeyboard(MoveDirection.Left);
            e.SuppressEvent = true;
        }
        else if (e.Data.KeyCode == KeyCode.VcUp && WIN())
        {
            MoveActiveWindowByKeyboard(MoveDirection.Up);
            e.SuppressEvent = true;
        }
        else if (e.Data.KeyCode == KeyCode.VcDown && WIN())
        {
            MoveActiveWindowByKeyboard(MoveDirection.Down);
            e.SuppressEvent = true;
        }
    }

    private void HookManager_KeyUp(object? sender, KeyboardHookEventArgs e)
    {
        //Used for overriding the Windows default hotkeys
        while (keysDown.Contains(e.Data.KeyCode))
        {
            keysDown.Remove(e.Data.KeyCode);
        }
    }

    private bool WIN()
    {
        if (keysDown.Contains(KeyCode.VcLeftMeta) ||
            keysDown.Contains(KeyCode.VcRightMeta))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}