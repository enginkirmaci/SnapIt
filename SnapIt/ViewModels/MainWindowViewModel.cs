using System;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SnapIt.Library.Services;
using SnapIt.Resources;

namespace SnapIt.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly ISettingService settingService;

        private bool HideWindowAtStartup = true;
        private bool isDarkTheme;

        public string Title { get => $"{Constants.AppName} {System.Windows.Forms.Application.ProductVersion}"; }
        public bool IsDarkTheme
        {
            get => isDarkTheme;
            set
            {
                SetProperty(ref isDarkTheme, value);
                settingService.Settings.IsDarkTheme = value;
                ModifyTheme(theme => theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light));
            }
        }

        public DelegateCommand<Window> ActivatedCommand { get; private set; }
        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        public DelegateCommand<string> HandleLinkClick { get; private set; }
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand LoadedCommand { get; private set; }

        public MainWindowViewModel(
            IRegionManager regionManager,
            ISnapService snapService,
            ISettingService settingService)
        {
            this.regionManager = regionManager;
            this.settingService = settingService;

            IsDarkTheme = settingService.Settings.IsDarkTheme;

            ActivatedCommand = new DelegateCommand<Window>((window) =>
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
            });

            LoadedCommand = new DelegateCommand(() =>
            {
                NavigateCommand.Execute("LayoutView");
            });

            CloseWindowCommand = new DelegateCommand<Window>((window) =>
            {
                if (window != null)
                {
                    settingService.Save();

                    window.Hide();
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

            if (!DevMode.IsActive)
            {
                snapService.Initialize();
            }
        }

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}