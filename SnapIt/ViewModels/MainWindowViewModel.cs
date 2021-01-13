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
        private bool isRunning;
        private string status;
        private bool isDarkTheme;
        private string themeTitle;

        public string Title { get => Constants.AppTitle; }
        public bool IsRunning { get => isRunning; set => SetProperty(ref isRunning, value); }
        public string Status { get => status; set => SetProperty(ref status, value); }
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
        public bool IsVersion3000MessageShown { get => settingService.Settings.IsVersion3000MessageShown; set { settingService.Settings.IsVersion3000MessageShown = value; } }

        public DelegateCommand<Window> ActivatedCommand { get; private set; }
        public DelegateCommand<Window> CloseWindowCommand { get; private set; }
        public DelegateCommand StartStopCommand { get; private set; }
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
                if (IsVersion3000MessageShown)
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

            LoadedCommand = new DelegateCommand(() =>
            {
                NavigateCommand.Execute("LayoutView");
            });

            StartStopCommand = new DelegateCommand(() =>
            {
                if (IsRunning)
                {
                    snapService.Release();
                }
                else
                {
                    snapService.Initialize();
                }
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

        private static void ModifyTheme(Action<ITheme> modificationAction)
        {
            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}