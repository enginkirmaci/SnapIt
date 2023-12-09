using System.Windows.Interop;
using DryIoc;
using SnapIt.Application.Contracts;
using SnapIt.Common;

namespace SnapIt.Application;

public class ScreenManager : IScreenManager
{
    private const uint WM_DISPLAYCHANGE = 126;
    private const uint WM_SETTINGCHANGE = 26;
    private readonly IContainer container;
    private static bool screenChanged = false;

    private ISnapManager snapService;

    public bool IsInitialized { get; private set; }

    public ScreenManager(IContainer container)
    {
        this.container = container;
    }

    public async Task InitializeAsync()
    {
        snapService = container.GetService<ISnapManager>();
        HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(System.Windows.Application.Current.MainWindow).Handle);
        source.AddHook(new HwndSourceHook(WndProc));
    }

    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        switch ((uint)msg)
        {
            case WM_DISPLAYCHANGE:
                Dev.Log("WM_DISPLAYCHANGE");
                screenChanged = true;
                ScreenChangedTask();

                break;

            case WM_SETTINGCHANGE:
                //screenChanged = true;
                //ScreenChangedTask(snapService);

                Dev.Log("WM_SETTINGCHANGE");

                break;
        }

        return nint.Zero;
    }

    private async void ScreenChangedTask()
    {
        //await Task.Delay(5000);

        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            if (screenChanged)
            {
                screenChanged = false;
                snapService.ScreenChangedEvent();
            }
        });
    }
}