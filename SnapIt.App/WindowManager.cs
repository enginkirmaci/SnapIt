using SnapIt.Application.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;
using SnapIt.Controls;
using SnapIt.Services.Contracts;

namespace SnapIt.Application;

public class WindowManager : IWindowManager
{
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private readonly IMouseService mouseService;
    private readonly IKeyboardService keyboardService;
    private List<SnapWindow> snapWindows;
    public bool IsInitialized { get; private set; }

    public WindowManager(
        ISettingService settingService,
        IWinApiService winApiService,
        IMouseService mouseService,
        IKeyboardService keyboardService
        )
    {
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.mouseService = mouseService;
        this.keyboardService = keyboardService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        await settingService.InitializeAsync();
        await winApiService.InitializeAsync();
        await mouseService.InitializeAsync();
        await keyboardService.InitializeAsync();

        mouseService.HideWindows += MouseService_HideWindows;
        mouseService.ShowWindowsIfNecessary += MouseService_ShowWindowsIfNecessary;
        mouseService.SelectElementWithPoint += MouseService_SelectElementWithPoint;

        keyboardService.GetSnapAreaBoundries += KeyboardService_GetSnapAreaBoundries;

        snapWindows = [];

        foreach (var screen in settingService.SnapScreens)
        {
            if (screen.IsActive)
            {
                var window = new SnapWindow(settingService, winApiService, screen);

                if (!snapWindows.Any(i => i.Screen == screen))
                {
                    window.ApplyLayout();

                    snapWindows.Add(window);
                }
            }
        }

        snapWindows.ForEach(window =>
        {
            window.Opacity = 0;
            window.Show();
            window.GenerateSnapAreaBoundries();
            window.Hide();
            window.Opacity = 100;
        });

        IsInitialized = true;
    }

    private void MouseService_HideWindows()
    {
        Hide();
    }

    private bool MouseService_ShowWindowsIfNecessary()
    {
        if (!snapWindows.TrueForAll(window => window.IsVisible))
        {
            Show();

            return true;
        }

        return false;
    }

    private SnapAreaInfo MouseService_SelectElementWithPoint(int x, int y)
    {
        var result = new SnapAreaInfo();

        foreach (var window in snapWindows)
        {
            var selectedArea = window.SelectElementWithPoint(x, y);
            if (!selectedArea.Equals(Rectangle.Empty))
            {
                result.Rectangle = selectedArea;
                result.Screen = window.Screen;

                break;
            }
        }

        return result;
    }

    private IList<Rectangle> KeyboardService_GetSnapAreaBoundries()
    {
        var boundries = new List<Rectangle>();

        snapWindows.ForEach(window => boundries.AddRange(window.SnapAreaBoundries));

        return boundries;
    }

    public void Release()
    {
        if (snapWindows != null && snapWindows.Count != 0)
        {
            for (int i = 0; i < snapWindows.Count; i++)
            {
                try
                {
                    snapWindows[i].Close();
                }
                catch { }
            }

            snapWindows.Clear();
        }

        IsInitialized = false;
    }

    public void Show()
    {
        snapWindows.ForEach(window =>
        {
            window.Show();
            window.Activate();
        });
    }

    public void Hide()
    {
        snapWindows.ForEach(window =>
        {
            window.Hide();
        });
    }

    public Dictionary<int, Rectangle> GetSnapAreaRectangles(SnapScreen snapScreen)
    {
        var window = snapWindows.FirstOrDefault(window => window.Screen.DeviceName == snapScreen.DeviceName);

        if (window != null)
        {
            return window.SnapAreaRectangles;
        }

        return null;
    }
}