using System.Windows;
using SnapIt.Application.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Mvvm;
using SnapIt.Services.Contracts;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace SnapIt.ViewModels.Pages;

public class SettingsPageViewModel : ViewModelBase
{
    private readonly ISettingService settingService;
    private readonly IThemeService themeService;
    private bool isStartupTaskActive;
    private UITheme selectedTheme;

    private bool isStandalone;

    public ObservableCollection<UITheme> ThemeList { get; set; }

    public UITheme SelectedTheme
    {
        get
        {
            selectedTheme = settingService.Settings.AppTheme;
            return selectedTheme;
        }
        set
        {
            settingService.Settings.AppTheme = value;
            SetProperty(ref selectedTheme, value);
            ChangeTheme();
        }
    }

    public bool IsRunAsAdmin
    { get => settingService.Settings.RunAsAdmin; set { settingService.Settings.RunAsAdmin = value; } }

    public bool IsStartupTaskActive
    {
        get => isStartupTaskActive;
        set
        {
            SetProperty(ref isStartupTaskActive, value);
            settingService.SetStartupTaskStatusAsync(value);
        }
    }

    public bool IsMinimizeTray
    { get => !settingService.Settings.ShowMainWindow; set { settingService.Settings.ShowMainWindow = !value; } }

    public bool CheckForNewVersion
    { get => settingService.Settings.CheckForNewVersion; set { settingService.Settings.CheckForNewVersion = value; } }

    public bool IsStandalone { get => isStandalone; set => SetProperty(ref isStandalone, value); }

    public SettingsPageViewModel(
        ISnapManager snapManager,
        ISettingService settingService,
        IThemeService themeService)
    {
        this.settingService = settingService;
        this.themeService = themeService;
        ThemeList = [
            UITheme.Light,
            UITheme.Dark,
            UITheme.System
        ];

#if STANDALONE
                    IsStandalone = true;
#endif
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        await settingService.InitializeAsync();

        IsStartupTaskActive = await settingService.GetStartupTaskStatusAsync();
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
}