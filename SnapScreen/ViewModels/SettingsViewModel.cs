using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using SnapScreen.Library.Entities;
using SnapScreen.Library.Services;
using WPFUI.Theme;

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
            get => SnapScreen.Properties.Settings.Default.RunAsAdmin;
            set
            {
                SnapScreen.Properties.Settings.Default.RunAsAdmin = value;
                SnapScreen.Properties.Settings.Default.Save();
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
                    Manager.Switch(Style.Dark, true);
                    break;

                case UITheme.Light:
                    Manager.Switch(Style.Light, true);
                    break;

                case UITheme.System:
                    Manager.SetSystemTheme(true);

                    break;
            }
        }
    }
}