using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Prism.Commands;
using SnapIt.Common;
using SnapIt.Common.Mvvm;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace Snap.Launcher.ViewModels.Windows;

public class MainWindowViewModel : ViewModelBase
{
    private readonly INavigationService navigationService;

    //private readonly ISettingService settingService;
    //private readonly IStoreLicenseService storeLicenseService;
    private readonly IThemeService themeService;

    //private readonly INotifyIconService notifyIconService;
    private readonly IContentDialogService contentDialogService;

    private ObservableCollection<object> menuItems;
    private bool isStandalone;
    private bool isTrial;
    private bool isTrialEnded;
    private bool isRunning;
    private Window mainWindow;

    public ObservableCollection<object> MenuItems { get => menuItems; set => SetProperty(ref menuItems, value); }
    public bool IsTrial { get => isTrial; set => SetProperty(ref isTrial, value); }
    public bool IsTrialEnded { get => isTrialEnded; set => SetProperty(ref isTrialEnded, value); }
    public bool IsRunning { get => isRunning; set => SetProperty(ref isRunning, value); }

    public DelegateCommand<CancelEventArgs> ClosingWindowCommand { get; private set; }
    public DelegateCommand TrialVersionCommand { get; private set; }

    public MainWindowViewModel(
        INavigationService navigationService,
        //ISettingService settingService,
        //IStoreLicenseService storeLicenseService,
        IThemeService themeService,
        //INotifyIconService notifyIconService,
        IContentDialogService contentDialogService)
    {
        this.navigationService = navigationService;
        //this.settingService = settingService;
        //this.storeLicenseService = storeLicenseService;
        this.themeService = themeService;
        //this.notifyIconService = notifyIconService;
        this.contentDialogService = contentDialogService;

        MenuItems =
        [
            //new NavigationViewItem("Home", SymbolRegular.Home24, typeof(DashboardPage)),
            //new NavigationViewItemSeparator(),
            //new NavigationViewItem("Layout", SymbolRegular.DataTreemap24, typeof(LayoutPage)),
            //new NavigationViewItem()
            //{
            //    Content = "Mouse",
            //    Icon = new FontIcon { Glyph = "", FontFamily = new FontFamily("Segoe Fluent Icons") },
            //    TargetPageType = typeof(MouseSettingsPage)
            //},
            //new NavigationViewItem("Keyboard", SymbolRegular.Keyboard24, typeof(KeyboardSettingsPage)),
            //new NavigationViewItem("Window", SymbolRegular.CalendarMultiple24, typeof(WindowsPage)),
            //new NavigationViewItem("Theme", SymbolRegular.Color24, typeof(ThemePage)),
            //new NavigationViewItem()
            //{
            //    Content = "Tutorials",
            //    Icon = new FontIcon { Glyph = "", FontFamily = new FontFamily("Segoe Fluent Icons") },
            //    TargetPageType = typeof(TutorialsPage)
            //},
            //new NavigationViewItem("Settings", SymbolRegular.Settings24, typeof(SettingsPage)),
            //new NavigationViewItem()
            //{
            //    Content = "About",
            //    Icon = new SymbolIcon { Symbol = SymbolRegular.Info24 },
            //    TargetPageType = typeof(AboutPage),
            //    MenuItems = new object[]
            //    {
            //        new NavigationViewItem("What's New", typeof(WhatsNewPage))
            //    }
            //}
        ];

#if !STANDALONE
        isStandalone = false;
#endif
#if STANDALONE
        isStandalone = true;
#endif

        //storeLicenseService.OfflineLicensesChanged += StoreLicenseService_OfflineLicensesChanged;

        //TrialVersionCommand = new DelegateCommand(async () => await OpenTrialMessageDialog());

        ClosingWindowCommand = new DelegateCommand<CancelEventArgs>((args) =>
        {
            args.Cancel = true;

            if (mainWindow != null)
            {
                mainWindow.Hide();
            }

            if (Dev.IsActive)
            {
                System.Windows.Application.Current.Shutdown();
            }
        });
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        mainWindow = (Window)args.Source;
        //if (!settingService.Settings.ShowMainWindow && !Dev.IsActive)
        //{
        //    mainWindow.Visibility = Visibility.Hidden;
        //}
        //else
        //{
        //    mainWindow.Visibility = Visibility.Visible;
        //}

        //navigationService.Navigate(typeof(DashboardPage));

        //ChangeTheme();

        //if (isStandalone)
        //{
        //    CheckForNewVersion();
        //}
        //else
        //{
        //    if (!Dev.SkipLicense)
        //    {
        //        storeLicenseService.Init(mainWindow);
        //    }
        //}

        //_ = CheckIfTrialAsync();
    }

    private void ChangeTheme()
    {
        //switch (settingService.Settings.AppTheme)
        //{
        //    case UITheme.Dark:
        //        themeService.SetTheme(ApplicationTheme.Dark);

        //        break;

        //    case UITheme.Light:
        //        themeService.SetTheme(ApplicationTheme.Light);

        //        break;

        //    case UITheme.System:
        //        var system = themeService.GetSystemTheme();
        //        themeService.SetTheme(system);

        //        break;
        //}

        SystemThemeWatcher.Watch(System.Windows.Application.Current.MainWindow);
    }
}