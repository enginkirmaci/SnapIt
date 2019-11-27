using System;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SnapIt.Library;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly ISettingService settingService;

        private bool HideWindowAtStartup = true;
        private bool isDarkTheme;
        private string themeTitle;

        public string Title { get => $"{Constants.AppName} {System.Windows.Forms.Application.ProductVersion}"; }
        public bool IsDarkTheme
        {
            get => isDarkTheme;
            set
            {
                ThemeTitle = value ? "Dark" : "Light";

                SetProperty(ref isDarkTheme, value);
                settingService.Settings.IsDarkTheme = value;
                ModifyTheme(theme => theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light));
            }
        }

        public string ThemeTitle { get => themeTitle; set => SetProperty(ref themeTitle, value); }
        public DelegateCommand<Window> ActivatedCommand { get; private set; }
        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        public DelegateCommand<string> HandleLinkClick { get; private set; }
        public DelegateCommand<string> NavigateCommand { get; private set; }
        public DelegateCommand LoadedCommand { get; private set; }

        public MainWindowViewModel(
            IWindowService windowService,
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
                NavigateCommand.Execute("ThemeView");
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
            else if (DevMode.ShowSnapWindowOnStartup)
            {
                windowService.Initialize();
                windowService.Show();
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