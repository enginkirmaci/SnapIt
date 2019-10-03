using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;

namespace SnapIt.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        private readonly ISnapService snapService;
        private readonly ISettingService settingService;

        private bool isStartupTaskActive;
        private ObservableCollection<MouseButton> mouseButtons;
        private ObservableCollection<HoldKey> holdKeys;

        public bool EnableKeyboard { get => settingService.Settings.EnableKeyboard; set { settingService.Settings.EnableKeyboard = value; ApplyChanges(); } }
        public bool EnableMouse { get => settingService.Settings.EnableMouse; set { settingService.Settings.EnableMouse = value; ApplyChanges(); } }
        public bool DragByTitle { get => settingService.Settings.DragByTitle; set { settingService.Settings.DragByTitle = value; ApplyChanges(); } }
        public MouseButton MouseButton { get => settingService.Settings.MouseButton; set { settingService.Settings.MouseButton = value; ApplyChanges(); } }
        public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }
        public bool EnableHoldKey { get => settingService.Settings.EnableHoldKey; set { settingService.Settings.EnableHoldKey = value; ApplyChanges(); } }
        public HoldKey HoldKey { get => settingService.Settings.HoldKey; set { settingService.Settings.HoldKey = value; ApplyChanges(); } }
        public ObservableCollection<HoldKey> HoldKeys { get => holdKeys; set => SetProperty(ref holdKeys, value); }
        public bool DisableForFullscreen { get => settingService.Settings.DisableForFullscreen; set { settingService.Settings.DisableForFullscreen = value; ApplyChanges(); } }
        public string MoveUpShortcut { get => settingService.Settings.MoveUpShortcut; set { settingService.Settings.MoveUpShortcut = value; ApplyChanges(); } }
        public string MoveDownShortcut { get => settingService.Settings.MoveDownShortcut; set { settingService.Settings.MoveDownShortcut = value; ApplyChanges(); } }
        public string MoveLeftShortcut { get => settingService.Settings.MoveLeftShortcut; set { settingService.Settings.MoveLeftShortcut = value; ApplyChanges(); } }
        public string MoveRightShortcut { get => settingService.Settings.MoveRightShortcut; set { settingService.Settings.MoveRightShortcut = value; ApplyChanges(); } }

        //public bool IsRunAsAdmin
        //{
        //	get => Properties.Settings.Default.RunAsAdmin;
        //	set
        //	{
        //		Properties.Settings.Default.RunAsAdmin = value;
        //		Properties.Settings.Default.Save();
        //	}
        //}

        public bool IsStartupTaskActive
        {
            get => isStartupTaskActive;
            set
            {
                SetProperty(ref isStartupTaskActive, value);
                settingService.SetStartupTaskStatusAsync(value);
            }
        }

        public DelegateCommand LoadedCommand { get; private set; }

        public SettingsViewModel(
            ISnapService snapService,
            ISettingService settingService)
        {
            this.snapService = snapService;
            this.settingService = settingService;

            LoadedCommand = new DelegateCommand(async () =>
            {
                IsStartupTaskActive = await settingService.GetStartupTaskStatusAsync();
            });

            MouseButtons = new ObservableCollection<MouseButton>
            {
                MouseButton.Left,
                MouseButton.Middle,
                MouseButton.Right
            };

            HoldKeys = new ObservableCollection<HoldKey> {
                HoldKey.Control,
                HoldKey.Alt,
                HoldKey.Shift,
                HoldKey.Win
            };
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