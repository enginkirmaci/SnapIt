using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;
using SnapIt.Library.Services;
using SnapIt.Views;
using Wpf.Ui.Controls;

namespace SnapIt.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ISettingService settingService;
        private readonly ISnapService snapService;
        private readonly IStandaloneLicenseService standaloneLicenseService;
        private readonly IStoreLicenseService storeLicenseService;
        private readonly IScreenChangeService screenChangeService;
        private bool isStandalone;
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
        private string notifyStatus = "Opening";
        private string licenseText;
        private Window mainWindow;

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
        public string NotifyStatus { get => notifyStatus; set => SetProperty(ref notifyStatus, value); }
        public string LicenseText { get => licenseText; set => SetProperty(ref licenseText, value); }

        public DelegateCommand<Window> LoadedCommand { get; private set; }
        public DelegateCommand<CancelEventArgs> ClosingWindowCommand { get; private set; }
        public DelegateCommand NotifyIconClickViewCommand { get; private set; }
        public DelegateCommand<string> NotifyIconDoubleClickViewCommand { get; private set; }

        //public DelegateCommand<object> NotifyIconOpenedCommand { get; private set; }
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
                ISettingService settingService,
                ISnapService snapService,
                IWindowService windowService,
                IStandaloneLicenseService standaloneLicenseService,
                IStoreLicenseService storeLicenseService,
                IScreenChangeService screenChangeService)
        {
            this.settingService = settingService;
            this.snapService = snapService;
            this.standaloneLicenseService = standaloneLicenseService;
            this.storeLicenseService = storeLicenseService;
            this.screenChangeService = screenChangeService;

#if !STANDALONE
            isStandalone = false;
#endif
#if STANDALONE
            isStandalone = true;
#endif

            snapService.StatusChanged += SnapService_StatusChanged;
            //snapService.ScreenLayoutLoaded += SnapService_ScreenLayoutLoaded;
            snapService.LayoutChanged += SnapService_LayoutChanged;
            storeLicenseService.OfflineLicensesChanged += StoreLicenseService_OfflineLicensesChanged;

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

                ChangeTheme();

                screenChangeService.Init(mainWindow);

                if (!settingService.Settings.ShowMainWindow && !DevMode.IsActive)
                {
                    mainWindow.Visibility = Visibility.Hidden;
                }

                if (isStandalone)
                {
                    if (!DevMode.SkipLicense)
                    {
                        CheckForNewVersion();
                    }
                }
                else
                {
                    if (!DevMode.SkipLicense)
                    {
                        storeLicenseService.Init(mainWindow);
                    }
                }

                CheckIfTrialAsync();
            });

            TrialVersionCommand = new DelegateCommand(() =>
            {
                IsTrialMessageOpen = true;
            });

            TrialMessageClosingCommand = new DelegateCommand<object>((isConfirm) =>
            {
                if ((bool)isConfirm)
                {
                    if (isStandalone)
                    {
                        PurchaseFullLicenseStandalone();
                    }
                    else
                    {
                        //if (SnapIt.Properties.Settings.Default.RunAsAdmin)
                        //{
                        //    Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox();
                        //    messageBox.ButtonRightName = LicenseMessageCloseButtonText;
                        //    messageBox.ButtonRightClick += MessageBox_ButtonRightClick;
                        //    messageBox.ButtonLeftClick += MessageBox_ButtonLeftClick;

                        //    messageBox.Show(
                        //        "Run as admin limitation!",
                        //        @"Due to limitations of Microsoft Store, \n
                        //            store purchases couldn't make while application is running as administrator.\n\n
                        //            Do you want start SnapIt without admin priviligies?");
                        //}

                        PurchaseFullLicense();
                    }

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

            LicenseMessageClosingCommand = new DelegateCommand<object>(async (isConfirm) =>
            {
                if ((bool)isConfirm)
                {
                    var isVerified = standaloneLicenseService.VerifyLicenseKey(LicenseMessageLicenseText?.Trim());

                    if (isVerified)
                    {
                        await CheckIfTrialAsync();

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
                if ((bool)isConfirm)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?ProductId={Constants.AppStoreId}"));
                }
                else
                {
                    if (IsTrialEnded)
                    {
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        IsTryStoreMessageOpen = false;
                    }
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

            NotifyIconClickViewCommand = new DelegateCommand(() =>
            {
                var popupWindow = new PopupWindow();

                popupWindow.Show();
                popupWindow.Activate();
                popupWindow.Focus();
            });

            NotifyIconDoubleClickViewCommand = new DelegateCommand<string>((navigatePath) =>
            {
                if (mainWindow != null && mainWindow.IsVisible)
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

            //NotifyIconOpenedCommand = new DelegateCommand<object>((obj) =>
            //{
            //    var rootTitleBar = mainWindow.FindChild<TitleBar>("RootTitleBar");

            //    var layoutsMenu = rootTitleBar.Tray.Menu.FindChild<MenuItem>("LayoutMenuItem");

            //    if (layoutsMenu == null)
            //    {
            //        return;
            //    }

            //    layoutsMenu.Items.Clear();

            //    var snapScreens = settingService.SnapScreens;
            //    var layouts = settingService.Layouts;

            //    foreach (var screen in snapScreens)
            //    {
            //        var screenMenu = new MenuItem()
            //        {
            //            Header = $"Display {screen.DeviceNumber} ({screen.Resolution}) - {screen.Primary}"
            //        };

            //        if (snapScreens.Count > 1)
            //        {
            //            layoutsMenu.Items.Add(screenMenu);
            //        }

            //        foreach (var layout in layouts)
            //        {
            //            var layoutMenuItem = new MenuItem()
            //            {
            //                Header = layout.Name,
            //                Tag = screen.DeviceName,
            //                Uid = layout.Guid.ToString(),
            //                SymbolIcon = Wpf.Ui.Common.SymbolRegular.Empty
            //            };
            //            layoutMenuItem.Click += LayoutItem_Click;

            //            if (screen.Layout == layout)
            //            {
            //                layoutMenuItem.SymbolIcon = Wpf.Ui.Common.SymbolRegular.Checkmark24;
            //            }
            //            else
            //            {
            //                layoutMenuItem.Header = "     " + layout.Name;
            //            }

            //            if (snapScreens.Count > 1)
            //            {
            //                screenMenu.Items.Add(layoutMenuItem);
            //            }
            //            else
            //            {
            //                layoutsMenu.Items.Add(layoutMenuItem);
            //            }
            //        }
            //    }
            //});

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

        //private void MessageBox_ButtonRightClick(object sender, RoutedEventArgs e)
        //{
        //    if (IsTrialEnded)
        //    {
        //        Application.Current.Shutdown();
        //    }
        //    else
        //    {
        //        ((Wpf.Ui.Controls.MessageBox)sender).Hide();
        //    }
        //}

        //private void MessageBox_ButtonLeftClick(object sender, RoutedEventArgs e)
        //{
        //    RunAsAdministrator.RunNormal();
        //}

        public void NavigateView(string navigatePath)
        {
            var rootNavigation = mainWindow.FindChild<NavigationStore>("RootNavigation");

            var index = 0;
            if (rootNavigation != null)
            {
                foreach (var item in rootNavigation.Items)
                {
                    if (item is NavigationItem navigationItem && navigationItem.PageTag != null && navigationItem.PageTag.Equals(navigatePath))
                    {
                        rootNavigation.Navigate(navigationItem.PageTag);

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
                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark, Wpf.Ui.Appearance.BackgroundType.Mica, false, true);
                    break;

                case UITheme.Light:
                    Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light, Wpf.Ui.Appearance.BackgroundType.Mica, false, true);
                    break;

                case UITheme.System:
                    var system = Wpf.Ui.Appearance.Theme.GetSystemTheme();
                    switch (system)
                    {
                        case Wpf.Ui.Appearance.SystemThemeType.Light:
                        case Wpf.Ui.Appearance.SystemThemeType.Sunrise:
                        case Wpf.Ui.Appearance.SystemThemeType.Flow:
                            Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light, Wpf.Ui.Appearance.BackgroundType.Mica, true, true);
                            break;

                        case Wpf.Ui.Appearance.SystemThemeType.Dark:
                        case Wpf.Ui.Appearance.SystemThemeType.Glow:
                        case Wpf.Ui.Appearance.SystemThemeType.CapturedMotion:
                            Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark, Wpf.Ui.Appearance.BackgroundType.Mica, true, true);
                            break;
                    }

                    break;
            }
        }

        private void SnapService_StatusChanged(bool isRunning)
        {
            IsRunning = isRunning;

            if (isRunning)
            {
                Status = "Stop";
                NotifyStatus = "Running";
            }
            else
            {
                Status = "Start";
                NotifyStatus = "Stopped";
            }
        }

        private void LayoutItem_Click(object sender, EventArgs e)
        {
            var layoutMenuItem = sender as MenuItem;

            var selectedSnapScreen = settingService.SnapScreens.FirstOrDefault();

            if (settingService.SnapScreens.Count > 1)
            {
                var snapScreen = settingService.SnapScreens.FirstOrDefault(screen => screen.DeviceName == layoutMenuItem.Tag.ToString());

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

        private void SnapService_LayoutChanged(SnapScreen snapScreen, Layout layout)
        {
            ShowNotification("Layout changed", $"{layout.Name} layout is set to Display {snapScreen.DeviceNumber} ({snapScreen.Resolution})");
        }

        public void ShowNotification(string title, string message, int timeout = 1000, System.Windows.Forms.ToolTipIcon tipIcon = System.Windows.Forms.ToolTipIcon.None)
        {
            App.NotifyIcon.ShowBalloonTip(timeout, title, message, tipIcon);
            App.NotifyIcon.Visible = true;
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

        private async Task CheckIfTrialAsync()
        {
            LicenseStatus licenseStatus = isStandalone ?
                standaloneLicenseService.CheckStatus() :
                await storeLicenseService.CheckStatusAsync();

            if (DevMode.TestInTrial)
            {
                licenseStatus = LicenseStatus.InTrial;
            }

            switch (licenseStatus)
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
                    if (isStandalone)
                    {
                        LicenseText = $"licensed to {standaloneLicenseService.License.Name}";
                    }
                    break;
            }

            LicenseMessageCloseButtonText = IsTrialEnded ? "Exit Application" : "Close";
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

        private void StoreLicenseService_OfflineLicensesChanged()
        {
            CheckIfTrialAsync();
        }

        private async void PurchaseFullLicense()
        {
            var purchaseResult = await storeLicenseService.RequestPurchaseAsync();

            switch (purchaseResult)
            {
                case PurchaseStatus.AlreadyPurchased:
                    //await ((MetroWindow)mainWindow).ShowMessageAsync("", $"You already bought this app and have a fully-licensed version.");
                    break;

                case PurchaseStatus.Succeeded:
                    // License will refresh automatically using the StoreContext.OfflineLicensesChanged event
                    break;

                case PurchaseStatus.Error:
                    IsTryStoreMessageOpen = true;
                    break;
            }
        }
    }
}