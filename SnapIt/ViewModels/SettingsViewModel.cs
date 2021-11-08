using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private readonly IEnumerable<AccentColorData> accentColors = ThemeManager.Current.Themes
                                            .GroupBy(x => x.ColorScheme)
                                            .OrderBy(a => a.Key)
                                            .Select(a => new AccentColorData { Name = a.Key, Color = ((SolidColorBrush)a.First().ShowcaseBrush).Color })
                                            .ToList();

        private bool isStartupTaskActive;
        private AccentColorData selectedAccentColor;
        private UITheme selectedTheme;

        public AccentColorData SelectedAccentColor
        {
            get => selectedAccentColor;
            set
            {
                SetProperty(ref selectedAccentColor, value);
                settingService.Settings.AppAccentColor = selectedAccentColor;
                ChangeTheme();
            }
        }

        public IEnumerable<AccentColorData> AccentColors { get => accentColors; }
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
            get => Properties.Settings.Default.RunAsAdmin;
            set
            {
                Properties.Settings.Default.RunAsAdmin = value;
                Properties.Settings.Default.Save();
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

        public bool CheckForNewVersion { get => settingService.Settings.CheckForNewVersion; set { settingService.Settings.CheckForNewVersion = value; } }

        public DelegateCommand LoadedCommand { get; private set; }

        public SettingsViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;

            ThemeList = new ObservableCollection<UITheme> {
                UITheme.Light,
                UITheme.Dark,
                UITheme.System
            };

            SelectedAccentColor = AccentColors.FirstOrDefault(a => a.Name == settingService.Settings.AppAccentColor.Name);
            if (SelectedAccentColor == null)
            {
                SelectedAccentColor = AccentColors.ElementAt(2);
            }

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
                    ThemeManager.Current.ChangeThemeColorScheme(Application.Current, settingService.Settings.AppAccentColor.Name);

                    ThemeManager.Current.ChangeThemeBaseColor(Application.Current, "Dark");
                    break;

                case UITheme.Light:
                    ThemeManager.Current.ChangeThemeColorScheme(Application.Current, settingService.Settings.AppAccentColor.Name);

                    ThemeManager.Current.ChangeThemeBaseColor(Application.Current, "Light");
                    break;

                case UITheme.System:
                    ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncAll;
                    ThemeManager.Current.SyncTheme();
                    break;
            }
        }
    }
}