﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SnapScreen.Library;
using SnapScreen.Library.Entities;
using SnapScreen.Library.Extensions;
using SnapScreen.Library.Services;
using WPFUI.Controls;
using WPFUI.Theme;

#if !STANDALONE

using System.Windows.Interop;
using Windows.Services.Store;

#endif

namespace SnapScreen.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly ISettingService settingService;
        private readonly ISnapService snapService;
        private readonly IStandaloneLicenseService standaloneLicenseService;

        private bool isTrial;
        private bool isTrialEnded;
        private bool isTrialMessageOpen;
        private bool isLicenseMessageOpen;
        private string licenseMessageCloseButtonText;
        private string licenseMessageLicenseText;
        private string licenseMessageErrorText;
        private bool isLicenseSuccess;
        private AppVersion? latestVersion;
        private bool newVersionMessageOpen;
        private bool isTryStoreMessageOpen;
        private bool isRunning;
        private string status;
        private string licenseText;
        private Window mainWindow;
#if !STANDALONE
        private StoreContext storeContext = null;
#endif

        public bool IsTrial { get => isTrial; set => SetProperty(ref isTrial, value); }
        public bool IsTrialEnded { get => isTrialEnded; set => SetProperty(ref isTrialEnded, value); }
        public bool IsTrialMessageOpen { get => isTrialMessageOpen; set => SetProperty(ref isTrialMessageOpen, value); }
        public bool IsLicenseMessageOpen { get => isLicenseMessageOpen; set => SetProperty(ref isLicenseMessageOpen, value); }
        public string LicenseMessageCloseButtonText { get => licenseMessageCloseButtonText; set => SetProperty(ref licenseMessageCloseButtonText, value); }
        public string LicenseMessageLicenseText { get => licenseMessageLicenseText; set => SetProperty(ref licenseMessageLicenseText, value); }
        public string LicenseMessageErrorText { get => licenseMessageErrorText; set => SetProperty(ref licenseMessageErrorText, value); }
        public bool IsLicenseSuccess { get => isLicenseSuccess; set => SetProperty(ref isLicenseSuccess, value); }
        public bool NewVersionMessageOpen { get => newVersionMessageOpen; set => SetProperty(ref newVersionMessageOpen, value); }
        public bool IsTryStoreMessageOpen { get => isTryStoreMessageOpen; set => SetProperty(ref isTryStoreMessageOpen, value); }
        public bool IsRunning { get => isRunning; set => SetProperty(ref isRunning, value); }
        public string Status { get => status; set => SetProperty(ref status, value); }
        public string LicenseText { get => licenseText; set => SetProperty(ref licenseText, value); }

        public DelegateCommand<Window> LoadedCommand { get; private set; }
        public DelegateCommand<CancelEventArgs> ClosingWindowCommand { get; private set; }
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand<string> NotifyIconViewCommand { get; private set; }
        public DelegateCommand<object> NotifyIconOpenedCommand { get; private set; }
        public DelegateCommand<string> HandleLinkClick { get; private set; }
        public DelegateCommand RateReviewStoreClick { get; private set; }
        public DelegateCommand ExitApplicationCommand { get; private set; }
        public DelegateCommand StartStopCommand { get; private set; }
        public DelegateCommand TrialVersionCommand { get; private set; }
        public DelegateCommand<object> TrialMessageClosingCommand { get; private set; }
        public DelegateCommand<object> LicenseMessageClosingCommand { get; private set; }
        public DelegateCommand<object> NewVersionMessageClosingCommand { get; private set; }
        public DelegateCommand<object> TryStoreMessageClosingCommand { get; private set; }

        public MainWindowViewModel(
                IRegionManager regionManager,
                ISettingService settingService,
                ISnapService snapService,
                IWindowService windowService,
                IStandaloneLicenseService standaloneLicenseService)
        {
            this.regionManager = regionManager;
            this.settingService = settingService;
            this.snapService = snapService;
            this.standaloneLicenseService = standaloneLicenseService;

            snapService.StatusChanged += SnapService_StatusChanged;
            //snapService.ScreenLayoutLoaded += SnapService_ScreenLayoutLoaded;
            snapService.LayoutChanged += SnapService_LayoutChanged;

            if (!DevMode.IsActive)
            {
                snapService.Initialize();
            }
            else if (DevMode.ShowSnapWindowOnStartup)
            {
                windowService.Initialize();
                windowService.Show();
            }

            LoadedCommand = new DelegateCommand<Window>((window) =>
            {
                mainWindow = window;

                WPFUI.Background.Manager.Apply(mainWindow);
                ChangeTheme();

                var contentControl = window.FindChildren<ContentControl>("MainFrame");

                RegionManager.SetRegionName(contentControl, Constants.MainRegion);
                RegionManager.SetRegionManager(contentControl, regionManager);

                if (!settingService.Settings.ShowMainWindow && !DevMode.IsActive)
                {
                    mainWindow.Visibility = Visibility.Hidden;
                }
                else
                {
                    NavigateView("LayoutView");
                }

#if !STANDALONE
                CheckIfTrialAsync();
#endif

#if STANDALONE
                CheckIfTrialStandAlone();

                CheckForNewVersion();
#endif
            });

            TrialVersionCommand = new DelegateCommand(() =>
            {
                IsTrialMessageOpen = true;
            });

            TrialMessageClosingCommand = new DelegateCommand<object>((isConfirm) =>
            {
                if ((bool)isConfirm)
                {
#if !STANDALONE
                    PurchaseFullLicense();
#endif
#if STANDALONE
                    PurchaseFullLicenseStandalone();
#endif

                    IsTrialMessageOpen = false;
                }
                else
                {
                    if (IsTrialEnded)
                    {
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        IsTrialMessageOpen = false;
                    }
                }
            });

            LicenseMessageClosingCommand = new DelegateCommand<object>((isConfirm) =>
            {
                if ((bool)isConfirm)
                {
                    var isVerified = standaloneLicenseService.VerifyLicenseKey(LicenseMessageLicenseText?.Trim());

                    if (isVerified)
                    {
                        CheckIfTrialStandAlone();

                        IsLicenseSuccess = true;

                        IsLicenseMessageOpen = false;
                    }
                    else
                    {
                        LicenseMessageErrorText = $"The entered license key is not valid. Check your license key.";
                    }
                }
                else if (IsTrialEnded)
                {
                    Application.Current.Shutdown();
                }
                else
                {
                    IsLicenseMessageOpen = false;
                }
            });

            NewVersionMessageClosingCommand = new DelegateCommand<object>((isConfirm) =>
            {
                if ((bool)isConfirm && latestVersion != null)
                {
                    var uriToLaunch = string.Format("https://" + Constants.AppNewVersionUrl, latestVersion.Version);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = uriToLaunch,
                        UseShellExecute = true
                    });
                }

                NewVersionMessageOpen = false;
            });

            TryStoreMessageClosingCommand = new DelegateCommand<object>(async (isConfirm) =>
            {
                if ((bool)isConfirm && latestVersion != null)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?ProductId={Constants.AppStoreId}"));
                }
                else
                {
                    Application.Current.Shutdown();
                }

                NewVersionMessageOpen = false;
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

            NavigateCommand = new DelegateCommand<string>((navigatePath) =>
            {
                if (navigatePath != null)
                {
                    this.regionManager.RequestNavigate(Constants.MainRegion, navigatePath, NavigationCompleted);
                }
            });

            NotifyIconViewCommand = new DelegateCommand<string>((navigatePath) =>
            {
                if (mainWindow.IsVisible)
                {
                    if (mainWindow.WindowState == WindowState.Minimized)
                    {
                        mainWindow.WindowState = WindowState.Normal;
                    }

                    mainWindow.Activate();
                }
                else
                {
                    mainWindow.Show();

                    mainWindow.Activate();
                }

                NavigateView(navigatePath);
            });

            NotifyIconOpenedCommand = new DelegateCommand<object>((obj) =>
            {
                var rootTitleBar = mainWindow.FindChildren<TitleBar>("RootTitleBar");

                var layoutsMenu = rootTitleBar.NotifyIconMenu.FindChildren<MenuItem>("LayoutMenuItem");

                if (layoutsMenu == null)
                {
                    return;
                }

                layoutsMenu.Items.Clear();

                var snapScreens = settingService.SnapScreens;
                var layouts = settingService.Layouts;

                foreach (var screen in snapScreens)
                {
                    var screenMenu = new MenuItem()
                    {
                        //Name = screen.Base.DeviceName,
                        Header = $"Display {screen.DeviceNumber} ({screen.Resolution}) - {screen.Primary}"
                    };

                    if (snapScreens.Count > 1)
                    {
                        layoutsMenu.Items.Add(screenMenu);
                    }

                    foreach (var layout in layouts)
                    {
                        var layoutMenuItem = new MenuItem()
                        {
                            Header = layout.Name,
                            Tag = screen.Base.DeviceName,
                            Uid = layout.Guid.ToString(),
                            Icon = null
                        };
                        layoutMenuItem.Click += LayoutItem_Click;

                        if (screen.Layout == layout)
                        {
                            layoutMenuItem.Icon = WPFUI.Common.Icon.Checkmark24;
                        }

                        if (snapScreens.Count > 1)
                        {
                            screenMenu.Items.Add(layoutMenuItem);
                        }
                        else
                        {
                            layoutsMenu.Items.Add(layoutMenuItem);
                        }
                    }
                }
            });

            RateReviewStoreClick = new DelegateCommand(async () =>
            {
                try
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://review/?ProductId={Constants.AppStoreId}"));
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show("Rate and review only works for Windows 10 or later versions");
                }
            });

            HandleLinkClick = new DelegateCommand<string>((url) =>
            {
                string uriToLaunch = $"https://{url}";
                Process.Start(new ProcessStartInfo
                {
                    FileName = uriToLaunch,
                    UseShellExecute = true
                });
            });

            ExitApplicationCommand = new DelegateCommand(() =>
            {
                mainWindow.Close();

                Application.Current.Shutdown();
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
        }

        private void NavigationCompleted(NavigationResult navigationResult)
        {
            if (navigationResult.Error != null)
            {
                //throw navigationResult.Error;
            }
        }

        private void NavigateView(string navigatePath)
        {
            var rootNavigation = mainWindow.FindChildren<NavigationFluent>("RootNavigation");

            var index = 0;
            if (rootNavigation != null)
            {
                foreach (var item in rootNavigation.Items)
                {
                    if (item.CommandParameter != null && item.CommandParameter.Equals(navigatePath))
                    {
                        rootNavigation.Navigate(item.Tag.ToString());
                        regionManager.RequestNavigate(Constants.MainRegion, navigatePath, NavigationCompleted);

                        break;
                    }

                    index++;
                }
            }
        }

        private void ChangeTheme()
        {
            switch (settingService.Settings.AppTheme)
            {
                case UITheme.Dark:
                    Manager.Switch(WPFUI.Theme.Style.Dark, true);
                    break;

                case UITheme.Light:
                    Manager.Switch(WPFUI.Theme.Style.Light, true);
                    break;

                case UITheme.System:
                    Manager.SetSystemTheme(true);

                    break;
            }
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

        private void LayoutItem_Click(object sender, EventArgs e)
        {
            var layoutMenuItem = sender as MenuItem;

            var selectedSnapScreen = settingService.SnapScreens.FirstOrDefault();

            if (settingService.SnapScreens.Count > 1)
            {
                var snapScreen = settingService.SnapScreens.FirstOrDefault(screen => screen.Base.DeviceName == layoutMenuItem.Tag.ToString());

                if (snapScreen != null)
                {
                    selectedSnapScreen = snapScreen;
                }
            }

            var selectedLayout = settingService.Layouts.FirstOrDefault(layout => layout.Guid.ToString() == layoutMenuItem.Uid);

            SnapService_LayoutChanged(selectedSnapScreen, selectedLayout);

            settingService.LinkScreenLayout(selectedSnapScreen, selectedLayout);
            snapService.Release();
            snapService.Initialize();
        }

        private void SnapService_LayoutChanged(Library.Entities.SnapScreen snapScreen, Layout layout)
        {
            ShowNotification("Layout changed", $"{layout.Name} layout is set to Display {snapScreen.DeviceNumber} ({snapScreen.Resolution})");
        }

        public void ShowNotification(string title, string message, int timeout = 1000, System.Windows.Forms.ToolTipIcon tipIcon = System.Windows.Forms.ToolTipIcon.None)
        {
            var notification = new System.Windows.Forms.NotifyIcon()
            {
                Visible = true,
                Icon = System.Drawing.SystemIcons.Information
            };

            notification.ShowBalloonTip(timeout, title, message, tipIcon);
        }

        private async void CheckForNewVersion()
        {
            if (settingService.Settings.CheckForNewVersion)
            {
                try
                {
                    await Task.Delay(new TimeSpan(0, 3, 0));
                    var client = new HttpClient();
                    var url = $"https://{Constants.AppVersionCheckUrl}";

                    var result = await client.GetAsync(url);
                    var response = string.Empty;

                    if (result.IsSuccessStatusCode)
                    {
                        response = await result.Content.ReadAsStringAsync();

                        latestVersion = JsonConvert.DeserializeObject<AppVersion>(response);

                        if (latestVersion != null && System.Windows.Forms.Application.ProductVersion != latestVersion.Version)
                        {
                            NewVersionMessageOpen = true;

                            if (!mainWindow.IsVisible)
                            {
                                mainWindow.Show();
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void CheckIfTrialStandAlone()
        {
            LicenseMessageCloseButtonText = IsTrialEnded ? "Exit Application" : "Close";

            switch (standaloneLicenseService.CheckStatus())
            {
                case LicenseStatus.InTrial:
                    IsTrial = true;
                    IsTrialEnded = false;
                    snapService.SetIsTrialEnded(false);
                    break;

                case LicenseStatus.TrialEnded:
                    IsTrial = true;
                    IsTrialEnded = true;
                    snapService.SetIsTrialEnded(true);
                    if (!mainWindow.IsVisible)
                    {
                        mainWindow.Show();
                    }

                    IsTrialMessageOpen = true;

                    break;

                case LicenseStatus.Licensed:
                    IsTrial = false;
                    IsTrialEnded = false;
                    snapService.SetIsTrialEnded(false);
                    LicenseText = $"licensed to {standaloneLicenseService.License.Name}";
                    break;
            }
        }

        private void PurchaseFullLicenseStandalone()
        {
            IsLicenseMessageOpen = true;

            var uriToLaunch = $"http://{Constants.AppPurchaseUrl}";
            Process.Start(new ProcessStartInfo
            {
                FileName = uriToLaunch,
                UseShellExecute = true
            });

            LicenseMessageCloseButtonText = IsTrialEnded ? "Exit Application" : "Close";
        }

#if !STANDALONE

        private async void CheckIfTrialAsync()
        {
            storeContext = StoreContext.GetDefault();
            storeContext.OfflineLicensesChanged += OfflineLicensesChanged;

            IInitializeWithWindow initWindow = (IInitializeWithWindow)(object)storeContext;
            initWindow.Initialize(new WindowInteropHelper(mainWindow).Handle);

            await GetLicenseState();

            if (IsTrialEnded)
            {
                if (!mainWindow.IsVisible)
                {
                    mainWindow.Show();
                }

                IsTrialMessageOpen = true;
            }
        }

        private void OfflineLicensesChanged(StoreContext sender, object args)
        {
            GetLicenseState();
        }

        private async Task GetLicenseState()
        {
            var license = await storeContext.GetAppLicenseAsync();

            if (license.IsActive)
            {
                if (license.IsTrial)
                {
                    IsTrial = true;

                    int remainingTrialTime = (license.ExpirationDate - DateTime.Now).Days;

                    if (remainingTrialTime <= 0)
                    {
                        IsTrialEnded = true;

                        snapService.SetIsTrialEnded(true);
                    }
                }
                else
                {
                    IsTrial = false;
                    IsTrialEnded = false;
                    snapService.SetIsTrialEnded(false);
                }
            }
        }

        private async void PurchaseFullLicense()
        {
            var hasError = false;

            var purchaseResult = await storeContext.RequestPurchaseAsync(Constants.AppStoreId);

            if (purchaseResult.ExtendedError != null)
            {
                hasError = true;
            }

            switch (purchaseResult.Status)
            {
                case StorePurchaseStatus.AlreadyPurchased:
                    //await ((MetroWindow)mainWindow).ShowMessageAsync("", $"You already bought this app and have a fully-licensed version.");
                    break;

                case StorePurchaseStatus.Succeeded:
                    // License will refresh automatically using the StoreContext.OfflineLicensesChanged event
                    break;

                default:
                    hasError = true;
                    break;
            }

            if (hasError)
            {
                IsTryStoreMessageOpen = true;
            }
        }

#endif

        [ComImport]
        [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IInitializeWithWindow
        {
            void Initialize(IntPtr hwnd);
        }
    }
}