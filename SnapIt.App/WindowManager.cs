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

    private List<SnapWindow> snapWindows;
    public bool IsInitialized { get; private set; }

    public WindowManager(
        ISettingService settingService,
        IWinApiService winApiService
        )
    {
        this.settingService = settingService;
        this.winApiService = winApiService;
    }

    public bool IsVisible
    {
        get => snapWindows.TrueForAll(window => window.IsVisible);
    }

    public async Task InitializeAsync()
    {
        //if (IsInitialized) return;

        await settingService.InitializeAsync();
        await winApiService.InitializeAsync();

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
    }

    public void Release()
    {
        if (snapWindows != null && snapWindows.Count != 0)
        {
            snapWindows.ForEach(window =>
            {
                try
                {
                    window.Close();
                }
                catch { }
            });
            snapWindows.Clear();
        }
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

    public IList<Rectangle> SnapAreaBoundries()
    {
        var boundries = new List<Rectangle>();

        snapWindows.ForEach(window => boundries.AddRange(window.SnapAreaBoundries));

        return boundries;
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

    public SnapAreaInfo SelectElementWithPoint(int x, int y)
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
}