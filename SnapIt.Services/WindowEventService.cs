using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class WindowEventService : IWindowEventService
{
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private readonly IWindowsService windowsService;
    private nint hookHandle;
    private WinApiService.WinEventDelegate hookDelegate;
    private readonly HashSet<nint> processedWindows;
    private readonly object lockObject = new();

    public bool IsInitialized { get; private set; }
    public bool IsMonitoring { get; private set; }

    public WindowEventService(
        ISettingService settingService,
        IWinApiService winApiService,
        IWindowsService windowsService)
    {
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.windowsService = windowsService;
        this.processedWindows = new HashSet<nint>();
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

        IsInitialized = true;
    }

    public void StartMonitoring()
    {
        if (!IsInitialized || IsMonitoring)
        {
            return;
        }

        if (!settingService.Settings.EnableAutomaticWindowCornering)
        {
            return;
        }

        hookDelegate = WinEventProc;

        hookHandle = WinApiService.SetWinEventHook(
            WinApiService.EVENT_OBJECT_SHOW,
            WinApiService.EVENT_OBJECT_SHOW,
            nint.Zero,
            hookDelegate,
            0,
            0,
            WinApiService.WINEVENT_OUTOFCONTEXT | WinApiService.WINEVENT_SKIPOWNPROCESS);

        if (hookHandle != nint.Zero)
        {
            IsMonitoring = true;
            Dev.Log("Window event monitoring started");
        }
        else
        {
            Dev.Log("Failed to start window event monitoring");
        }
    }

    public void StopMonitoring()
    {
        if (!IsMonitoring)
        {
            return;
        }

        if (hookHandle != nint.Zero)
        {
            WinApiService.UnhookWinEvent(hookHandle);
            hookHandle = nint.Zero;
            IsMonitoring = false;
            Dev.Log("Window event monitoring stopped");
        }

        lock (lockObject)
        {
            processedWindows.Clear();
        }
    }

    private void WinEventProc(nint hWinEventHook, uint eventType, nint hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (idObject != WinApiService.OBJID_WINDOW || hwnd == nint.Zero)
        {
            return;
        }

        if (eventType == WinApiService.EVENT_OBJECT_SHOW)
        {
            Task.Run(() => ProcessNewWindow(hwnd));
        }
    }

    private void ProcessNewWindow(nint hwnd)
    {
        try
        {
            lock (lockObject)
            {
                if (processedWindows.Contains(hwnd))
                {
                    return;
                }
                processedWindows.Add(hwnd);
            }

            Thread.Sleep(100);

            if (!PInvoke.User32.IsWindowVisible(hwnd))
            {
                return;
            }

            var chars = 256;
            var buff = new char[chars + 1];
            var length = PInvoke.User32.GetWindowText(hwnd, buff, chars);

            if (length == 0)
            {
                return;
            }

            var windowTitle = new string(buff, 0, length);

            if (string.IsNullOrWhiteSpace(windowTitle) || windowTitle.Equals("Program Manager"))
            {
                return;
            }

            if (windowsService.IsExcludedApplication(windowTitle, false))
            {
                Dev.Log($"Window excluded from cornering: {windowTitle}");
                return;
            }

            var activeWindow = new ActiveWindow
            {
                Handle = hwnd,
                Title = windowTitle
            };

            if (PInvoke.User32.GetWindowRect(hwnd, out PInvoke.RECT rct))
            {
                activeWindow.Boundry = new Rectangle(rct.left, rct.top, rct.right, rct.bottom);
            }

            if (activeWindow.Boundry.Equals(Rectangle.Empty))
            {
                return;
            }

            winApiService.SetWindowCornerPreference(activeWindow, DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND);
        }
        catch (Exception ex)
        {
            Dev.Log($"Error processing new window: {ex.Message}");
        }
        finally
        {
            Task.Run(async () =>
            {
                await Task.Delay(30000);
                lock (lockObject)
                {
                    processedWindows.Remove(hwnd);
                }
            });
        }
    }

    public void Dispose()
    {
        StopMonitoring();
        IsInitialized = false;
    }
}