using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly ISettingService settingService;

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
        {
            get => SnapIt.Properties.Settings.Default.RunAsAdmin;
            set
            {
                SnapIt.Properties.Settings.Default.RunAsAdmin = value;
                SnapIt.Properties.Settings.Default.Save();
            }
        }

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
        public DelegateCommand LoadedCommand { get; private set; }

        public SettingsViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.settingService = settingService;

            ThemeList = new ObservableCollection<UITheme> {
                UITheme.Light,
                UITheme.Dark,
                UITheme.System
            };

#if STANDALONE
                        IsStandalone = true;
#endif

            LoadedCommand = new DelegateCommand(async () =>
            {
                IsStartupTaskActive = await settingService.GetStartupTaskStatusAsync();
            });
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
    }
}