using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SnapIt.Library;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.Services;
using Windows.Services.Store;

namespace SnapIt.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private bool HideWindowAtStartup = true;
        private bool isRunning;
        private string status;
        private bool isPaneOpen = true;
        private Window mainWindow;

        public string Title { get => Constants.AppTitle; }
        public bool IsRunning { get => isRunning; set => SetProperty(ref isRunning, value); }
        public string Status { get => status; set => SetProperty(ref status, value); }
        public bool IsPaneOpen { get => isPaneOpen; set => SetProperty(ref isPaneOpen, value); }
        //public bool IsVersion3000MessageShown { get => settingService.Settings.IsVersion3000MessageShown; set { settingService.Settings.IsVersion3000MessageShown = value; } }
        public ObservableCollection<UITheme> ThemeList { get; set; }

        public DelegateCommand<Window> ActivatedCommand { get; private set; }
        public DelegateCommand<Window> LoadedCommand { get; private set; }
        public DelegateCommand<CancelEventArgs> ClosingWindowCommand { get; private set; }
        public DelegateCommand StartStopCommand { get; private set; }
        public DelegateCommand<string> HandleLinkClick { get; private set; }
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand<object> ThemeItemCommand { get; }

        public MainWindowViewModel(
            IWindowService windowService,
            IRegionManager regionManager,
            ISnapService snapService,
            ISettingService settingService)
        {
            this.regionManager = regionManager;
            this.snapService = snapService;
            this.settingService = settingService;

            ThemeList = new ObservableCollection<UITheme> {
                UITheme.Light,
                UITheme.Dark,
                UITheme.System
            };

            snapService.StatusChanged += SnapService_StatusChanged;

            if (!DevMode.IsActive)
            {
                snapService.Initialize();
            }
            else if (DevMode.ShowSnapWindowOnStartup)
            {
                windowService.Initialize();
                windowService.Show();
            }

            ActivatedCommand = new DelegateCommand<Window>((window) =>
            {
                //CheckTrialExpiredAsync();

                //if (IsVersion3000MessageShown)
                {
                    if (settingService.Settings.ShowMainWindow)
                    {
                        settingService.Settings.ShowMainWindow = false;
                        HideWindowAtStartup = false;
                    }
                    else if (!DevMode.IsActive && HideWindowAtStartup)
                    {
                        HideWindowAtStartup = false;
                        window.Hide();
                    }
                }
            });

            LoadedCommand = new DelegateCommand<Window>((window) =>
            {
                mainWindow = window;

                //                if (!IsVersion3000MessageShown)
                //                {
                //                    ((MetroWindow)window).ShowMessageAsync("Important Notice !",
                //                        @"I have been getting lots of feedback about layouts and layout designer which tells how difficult or not possible to design layouts that you want. I was aware of it, but there were some difficulties needs to solved.

                //After some experimenting, I finally managed to develop more flexible and easier layout mechanism.This new mechanism opens more possibilities in the future.

                //Because of the changes, unfortunately your old layout can't work with this version and there is no way for me to migrate it to newer structure. I know that some of you spend a lot of time to create it and I'm really sorry.

                //Please give a try to new layout designer.I hope you'll enjoy it.
                //");
                //                    IsVersion3000MessageShown = true;
                //                }

                var contentControl = window.FindChildren<ContentControl>("MainContentControl");

                RegionManager.SetRegionName(contentControl, Constants.MainRegion);
                RegionManager.SetRegionManager(contentControl, regionManager);

                NavigateCommand.Execute("LayoutView");

                HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
                source.AddHook(new HwndSourceHook(WndProc));
            });

            StartStopCommand = new DelegateCommand(() =>
            {
                if (snapService.IsRunning)
                {
                    snapService.Release();
                }
                else
                {
                    snapService.Initialize();
                }
            });

            ClosingWindowCommand = new DelegateCommand<CancelEventArgs>((args) =>
            {
                args.Cancel = true;

                if (mainWindow != null)
                {
                    settingService.Save();

                    mainWindow.Hide();
                }

                if (DevMode.IsActive)
                {
                    Application.Current.Shutdown();
                }
            });

            HandleLinkClick = new DelegateCommand<string>(async (url) =>
            {
                string uriToLaunch = $"http://{url}";
                var uri = new Uri(uriToLaunch);

                await Windows.System.Launcher.LaunchUriAsync(uri);
            });

            NavigateCommand = new DelegateCommand<string>((navigatePath) =>
            {
                if (navigatePath != null)
                {
                    this.regionManager.RequestNavigate(Constants.MainRegion, navigatePath);
                }
            });

            ThemeItemCommand = new DelegateCommand<object>((theme) =>
            {
                switch ((UITheme)theme)
                {
                    case UITheme.Dark:
                        //ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAccent;
                        //ThemeManager.Current.SyncTheme();
                        ThemeManager.Current.ChangeThemeColorScheme(Application.Current, settingService.Settings.AppAccentColor.Name);
                        ThemeManager.Current.ChangeThemeBaseColor(Application.Current, "Dark");
                        break;

                    case UITheme.Light:
                        //ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAccent;
                        //ThemeManager.Current.SyncTheme();
                        ThemeManager.Current.ChangeThemeColorScheme(Application.Current, settingService.Settings.AppAccentColor.Name);
                        ThemeManager.Current.ChangeThemeBaseColor(Application.Current, "Light");
                        break;

                    case UITheme.System:
                        ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
                        ThemeManager.Current.SyncTheme();
                        break;
                }

                settingService.Settings.AppTheme = (UITheme)theme;
            });

            ThemeItemCommand.Execute(settingService.Settings.AppTheme);
        }

        private const uint WM_DISPLAYCHANGE = 0x007e;

        //TODO consider this to move SnapService
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((uint)msg)
            {
                case WM_DISPLAYCHANGE:

                    snapService.ScreenChangedEvent();

                    break;
            }
            return IntPtr.Zero;
        }

        private void SnapService_StatusChanged(bool isRunning)
        {
            IsRunning = isRunning;

            if (isRunning)
            {
                Status = "Stop";
            }
            else
            {
                Status = "Start";
            }
        }

        private StoreContext storeContext = null;
        private bool isStoreContextLoaded = false;

        private async void CheckTrialExpiredAsync()
        {
            if (mainWindow != null && mainWindow.IsActive && !isStoreContextLoaded)
            {
                isStoreContextLoaded = true;
                storeContext = StoreContext.GetDefault();
                //storeContext.OfflineLicensesChanged += OfflineLicensesChanged;

                IInitializeWithWindow initWindow = (IInitializeWithWindow)(object)storeContext;
                initWindow.Initialize(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);

                StoreAppLicense license = await storeContext.GetAppLicenseAsync();

                if (license.IsActive)
                {
                    //if (license.IsTrial)
                    {
                        var result = await ((MetroWindow)mainWindow).ShowMessageAsync(
                            "Trial Notice !",
                            @"Trial license",
                            MessageDialogStyle.AffirmativeAndNegative,
                            new MetroDialogSettings
                            {
                                AffirmativeButtonText = "Buy Full Version!"
                            });

                        if (result == MessageDialogResult.Affirmative)
                        {
                            PurchaseFullLicense();
                        }
                    }
                }
            }
        }

        private async void PurchaseFullLicense()
        {
            // Get app store product details
            StoreProductResult productResult = await storeContext.GetStoreProductForCurrentAppAsync();
            if (productResult.ExtendedError != null)
            {
                // The user may be offline or there might be some other server failure
                DevMode.Log($"ExtendedError: {productResult.ExtendedError.Message}");
                return;
            }

            DevMode.Log("Buying the full license...");
            StoreAppLicense license = await storeContext.GetAppLicenseAsync();
            if (license.IsTrial)
            {
                StorePurchaseResult result = await productResult.Product.RequestPurchaseAsync();
                if (result.ExtendedError != null)
                {
                    await ((MetroWindow)mainWindow).ShowMessageAsync("", $"Purchase failed: ExtendedError: {result.ExtendedError.Message}");
                    return;
                }

                switch (result.Status)
                {
                    case StorePurchaseStatus.AlreadyPurchased:
                        await ((MetroWindow)mainWindow).ShowMessageAsync("", $"You already bought this app and have a fully-licensed version.");
                        break;

                    case StorePurchaseStatus.Succeeded:
                        // License will refresh automatically using the StoreContext.OfflineLicensesChanged event
                        break;

                    case StorePurchaseStatus.NotPurchased:
                        await ((MetroWindow)mainWindow).ShowMessageAsync("", "Product was not purchased, it may have been canceled.");
                        break;

                    case StorePurchaseStatus.NetworkError:
                        await ((MetroWindow)mainWindow).ShowMessageAsync("", "Product was not purchased due to a Network Error.");
                        break;

                    case StorePurchaseStatus.ServerError:
                        await ((MetroWindow)mainWindow).ShowMessageAsync("", "Product was not purchased due to a Server Error.");
                        break;

                    default:
                        await ((MetroWindow)mainWindow).ShowMessageAsync("", "Product was not purchased due to an Unknown Error.");
                        break;
                }
            }
        }
    }

    [ComImport]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IInitializeWithWindow
    {
        void Initialize(IntPtr hwnd);
    }
}