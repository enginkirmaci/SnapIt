using System;
using System.Windows;
using System.Windows.Interop;

namespace SnapScreen.Library.Services
{
    public class ScreenChangeService : IScreenChangeService
    {
        private const uint WM_DISPLAYCHANGE = 126;
        private const uint WM_SETTINGCHANGE = 26;
        private static bool screenChanged = false;

        private ISnapService snapService { get; }

        public ScreenChangeService(
            ISnapService snapService)
        {
            this.snapService = snapService;
        }

        public void Init(Window window)
        {
            HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((uint)msg)
            {
                case WM_DISPLAYCHANGE:
                    screenChanged = true;
                    ScreenChangedTask(snapService);

                    break;

                    //case WM_SETTINGCHANGE:
                    //    screenChanged = true;
                    //    ScreenChangedTask(snapService);

                    //    DevMode.Log($"WM_SETTINGCHANGE change");

                    //    break;
            }

            return IntPtr.Zero;
        }

        private async void ScreenChangedTask(ISnapService snapService)
        {
            //await Task.Delay(5000);

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (screenChanged)
                {
                    screenChanged = false;
                    snapService.ScreenChangedEvent();
                }
            });
        }
    }

    public interface IScreenChangeService
    {
        void Init(Window window);
    }
}