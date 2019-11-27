using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class KeyboardSettingsViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private bool isHotKeyControlFocused;

        public bool EnableKeyboard { get => settingService.Settings.EnableKeyboard; set { settingService.Settings.EnableKeyboard = value; ApplyChanges(); } }

        public string MoveUpShortcut { get => settingService.Settings.MoveUpShortcut; set { settingService.Settings.MoveUpShortcut = value; ApplyChanges(); } }
        public string MoveDownShortcut { get => settingService.Settings.MoveDownShortcut; set { settingService.Settings.MoveDownShortcut = value; ApplyChanges(); } }
        public string MoveLeftShortcut { get => settingService.Settings.MoveLeftShortcut; set { settingService.Settings.MoveLeftShortcut = value; ApplyChanges(); } }
        public string MoveRightShortcut { get => settingService.Settings.MoveRightShortcut; set { settingService.Settings.MoveRightShortcut = value; ApplyChanges(); } }
        public bool IsHotKeyControlFocused
        {
            get => isHotKeyControlFocused;
            set
            {
                DevMode.Log($"focused => {value}");

                if (!DevMode.IsActive)
                {
                    if (value)
                    {
                        snapService.Release();
                    }
                    else
                    {
                        snapService.Initialize();
                    }
                }

                SetProperty(ref isHotKeyControlFocused, value);
            }
        }

        public DelegateCommand LoadedCommand { get; private set; }

        public KeyboardSettingsViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;
        }

        private void ApplyChanges()
        {
            if (!DevMode.IsActive)
            {
                snapService.Release();
                snapService.Initialize();
            }
        }
    }
}