using System.ComponentModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Media;
using DryIoc;
using Prism.Commands;
using Prism.Ioc;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Helpers;
using SnapIt.Common.Mvvm;
using SnapIt.Services;
using SnapIt.Services.Contracts;
using SnapIt.Views.Dialogs;
using SnapIt.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SnapIt.ViewModels.Windows;

public class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService navigationService;
    private readonly ISettingService settingService;
    private readonly ISnapManager snapManager;
    private readonly IStoreLicenseService storeLicenseService;
    private readonly IThemeService themeService;
    private readonly INotifyIconService notifyIconService;
    private readonly IContentDialogService contentDialogService;

    private ObservableCollection<object> menuItems;
    private bool isStandalone;
    private bool isTrial;
    private bool isTrialEnded;
    private bool isRunning;
    private string status;
    private Window mainWindow;

    public ObservableCollection<object> MenuItems { get => menuItems; set => SetProperty(ref menuItems, value); }
    public bool IsTrial { get => isTrial; set => SetProperty(ref isTrial, value); }
    public bool IsTrialEnded { get => isTrialEnded; set => SetProperty(ref isTrialEnded, value); }
    public bool IsRunning { get => isRunning; set => SetProperty(ref isRunning, value); }
    public string Status { get => status; set => SetProperty(ref status, value); }

    //public bool IsLicenseMessageOpen { get => isLicenseMessageOpen; set => SetProperty(ref isLicenseMessageOpen, value); }
    //public string LicenseText { get => licenseText; set => SetProperty(ref licenseText, value); }
    //public string LicenseMessageLicenseText { get => licenseMessageLicenseText; set => SetProperty(ref licenseMessageLicenseText, value); }
    //public string LicenseMessageErrorText { get => licenseMessageErrorText; set => SetProperty(ref licenseMessageErrorText, value); }
    //public bool IsLicenseSuccess { get => isLicenseSuccess; set => SetProperty(ref isLicenseSuccess, value); }

    public DelegateCommand<CancelEventArgs> ClosingWindowCommand { get; private set; }
    public DelegateCommand StartStopCommand { get; private set; }
    public DelegateCommand TrialVersionCommand { get; private set; }

    public MainWindowViewModel(
        INavigationService navigationService,
        ISettingService settingService,
        ISnapManager snapManager,
        IStoreLicenseService storeLicenseService,
        IThemeService themeService,
        INotifyIconService notifyIconService,
        IContentDialogService contentDialogService)
    {
        this.navigationService = navigationService;
        this.settingService = settingService;
        this.snapManager = snapManager;
        this.storeLicenseService = storeLicenseService;
        this.themeService = themeService;
        this.notifyIconService = notifyIconService;
        this.contentDialogService = contentDialogService;

        MenuItems =
        [
            new NavigationViewItem("Home", SymbolRegular.Home24, typeof(DashboardPage)),
            new NavigationViewItemSeparator(),
            new NavigationViewItem("Layout", SymbolRegular.DataTreemap24, typeof(LayoutPage)),
            new NavigationViewItem()
            {
                Content = "Mouse",
                Icon = new FontIcon { Glyph = "", FontFamily = new FontFamily("Segoe Fluent Icons") },
                TargetPageType = typeof(MouseSettingsPage)
            },
            new NavigationViewItem("Keyboard", SymbolRegular.Keyboard24, typeof(KeyboardSettingsPage)),
            new NavigationViewItem("Window", SymbolRegular.CalendarMultiple24, typeof(WindowsPage)),
            new NavigationViewItem("Theme", SymbolRegular.Color24, typeof(ThemePage)),
            new NavigationViewItem()
            {
                Content = "Tutorials",
                Icon = new FontIcon { Glyph = "", FontFamily = new FontFamily("Segoe Fluent Icons") },
                TargetPageType = typeof(TutorialsPage)
            },
            new NavigationViewItem("Settings", SymbolRegular.Settings24, typeof(SettingsPage)),
            new NavigationViewItem()
            {
                Content = "About",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Info24 },
                TargetPageType = typeof(AboutPage),
                MenuItems = new object[]
                {
                    new NavigationViewItem("What's New", typeof(WhatsNewPage))
                }
            }
        ];

#if !STANDALONE
        isStandalone = false;
#endif
#if STANDALONE
        isStandalone = true;
#endif

        snapManager.StatusChanged += SnapService_StatusChanged;
        snapManager.LayoutChanged += SnapService_LayoutChanged;
        storeLicenseService.OfflineLicensesChanged += StoreLicenseService_OfflineLicensesChanged;

        //LicenseMessageClosingCommand = new DelegateCommand<object>(async (isConfirm) =>
        //{
        //    if ((bool)isConfirm)
        //    {
        //        var isVerified = standaloneLicenseService.VerifyLicenseKey(LicenseMessageLicenseText?.Trim());

        //        if (isVerified)
        //        {
        //            await CheckIfTrialAsync();

        //            IsLicenseSuccess = true;

        //            IsLicenseMessageOpen = false;
        //        }
        //        else
        //        {
        //            LicenseMessageErrorText = $"The entered license key is not valid. Check your license key.";
        //        }
        //    }
        //    else if (IsTrialEnded)
        //    {
        //        Application.Current.Shutdown();
        //    }
        //    else
        //    {
        //        IsLicenseMessageOpen = false;
        //    }
        //});

        TrialVersionCommand = new DelegateCommand(async () => await OpenTrialMessageDialog());

        ClosingWindowCommand = new DelegateCommand<CancelEventArgs>((args) =>
        {
            args.Cancel = true;

            if (mainWindow != null)
            {
                settingService.Save();

                mainWindow.Hide();
            }

            if (Dev.IsActive)
            {
                System.Windows.Application.Current.Shutdown();
            }
        });

        StartStopCommand = new DelegateCommand(snapManager.StartStop);
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        if (!Dev.IsActive)
        {
            var snapManager = App.AppContainer.Resolve<ISnapManager>();
            await snapManager.InitializeAsync();

            //await TaskEx.WaitUntil(() => snapManager.IsInitialized);
        }

        await settingService.InitializeAsync();

        notifyIconService.InitializeAsync();

        mainWindow = (Window)args.Source;
        if (!settingService.Settings.ShowMainWindow && !Dev.IsActive)
        {
            mainWindow.Visibility = Visibility.Hidden;
        }
        else
        {
            mainWindow.Visibility = Visibility.Visible;
        }

        navigationService.Navigate(typeof(DashboardPage));

        ChangeTheme();

        if (isStandalone)
        {
            CheckForNewVersion();
        }
        else
        {
            if (!Dev.SkipLicense)
            {
                storeLicenseService.Init(mainWindow);
            }
        }

        _ = CheckIfTrialAsync();
    }

    private void ChangeTheme()
    {
        switch (settingService.Settings.AppTheme)
        {
            case UITheme.Dark:
                themeService.SetTheme(ApplicationTheme.Dark);

                break;

            case UITheme.Light:
                themeService.SetTheme(ApplicationTheme.Light);

                break;

            case UITheme.System:
                var system = themeService.GetSystemTheme();
                themeService.SetTheme(system);

                break;
        }

        SystemThemeWatcher.Watch(System.Windows.Application.Current.MainWindow);
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

    private void SnapService_LayoutChanged(SnapScreen snapScreen, Layout layout)
    {
        ShowNotification("Layout changed", $"{layout.Name} layout is set to Display {snapScreen.DeviceNumber} ({snapScreen.Resolution})");
    }

    public void ShowNotification(string title, string message, int timeout = 1000, System.Windows.Forms.ToolTipIcon tipIcon = System.Windows.Forms.ToolTipIcon.None)
    {
        //App.NotifyIcon.ShowBalloonTip(timeout, title, message, tipIcon);
        //App.NotifyIcon.Visible = true;
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

                    var latestVersion = Json.Deserialize<AppVersion>(response);

                    if (latestVersion != null && System.Windows.Application.ResourceAssembly.GetName().Version.ToString() != latestVersion.Version)
                    {
                        var newVersionDialog = new NewVersionDialog(contentDialogService.GetContentPresenter());

                        var newVersionResult = await newVersionDialog.ShowAsync();
                        if (newVersionResult == ContentDialogResult.Primary)
                        {
                            var uriToLaunch = string.Format("https://" + Constants.AppNewVersionUrl, latestVersion.Version);
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = uriToLaunch,
                                UseShellExecute = true
                            });
                        }

                        if (!mainWindow.IsVisible)
                        {
                            mainWindow.Show();
                        }
                    }
                }
            }
            catch { }
        }
    }

    public async Task OpenTrialMessageDialog()
    {
        var trialMessageDialog = new TrialMessageDialog(contentDialogService.GetContentPresenter());
        trialMessageDialog.ViewModel.IsTrialEnded = IsTrialEnded;

        var result = await trialMessageDialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            PurchaseFullLicense();
        }
        else if (IsTrialEnded)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }

    private async Task CheckIfTrialAsync()
    {
        LicenseStatus licenseStatus = !isStandalone ?
            await storeLicenseService.CheckStatusAsync()
            : LicenseStatus.Licensed;

        if (Dev.TestInTrial)
        {
            licenseStatus = LicenseStatus.InTrial;
        }
        else if (Dev.TestTrialEnded)
        {
            licenseStatus = LicenseStatus.TrialEnded;
        }

        switch (licenseStatus)
        {
            case LicenseStatus.InTrial:
                IsTrial = true;
                IsTrialEnded = false;
                snapManager.SetIsTrialEnded(false);
                break;

            case LicenseStatus.TrialEnded:
                IsTrial = true;
                IsTrialEnded = true;
                snapManager.SetIsTrialEnded(true);
                if (!mainWindow.IsVisible)
                {
                    mainWindow.Show();
                }

                await OpenTrialMessageDialog();

                break;

            case LicenseStatus.Licensed:
                IsTrial = false;
                IsTrialEnded = false;
                snapManager.SetIsTrialEnded(false);
                //if (isStandalone)
                //{
                //    LicenseText = $"licensed to {standaloneLicenseService.License.Name}";
                //}
                break;
        }
    }

    private void StoreLicenseService_OfflineLicensesChanged()
    {
        _ = CheckIfTrialAsync();
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
                await OpenStoreWebsiteDialog();
                break;
        }
    }

    public async Task OpenStoreWebsiteDialog()
    {
        var storeWebsiteDialog = new StoreWebsiteDialog(contentDialogService.GetContentPresenter());

        storeWebsiteDialog.ViewModel.IsTrialEnded = IsTrialEnded;
        var result = await storeWebsiteDialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await Launcher.LaunchUriAsync(new Uri($"ms-windows-store://pdp/?ProductId={Constants.AppStoreId}"));
        }
        else
        {
            if (IsTrialEnded)
            {
                System.Windows.Application.Current.Shutdown();
            }
        }
    }

    //private void PurchaseFullLicenseStandalone()
    //{
    //    IsLicenseMessageOpen = true;

    //    var uriToLaunch = $"http://{Constants.AppPurchaseUrl}";
    //    Process.Start(new ProcessStartInfo
    //    {
    //        FileName = uriToLaunch,
    //        UseShellExecute = true
    //    });

    //    LicenseMessageCloseButtonText = IsTrialEnded ? "Exit Application" : "Close";
    //}
}