﻿using System.Collections.ObjectModel;
using System.Linq;
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
        private ObservableCollection<Resource<HoldKeyBehaviour>> holdKeyBehaviours;
        private bool enableHoldKey;
        private bool isHotKeyControlFocused;

        public bool EnableKeyboard { get => settingService.Settings.EnableKeyboard; set { settingService.Settings.EnableKeyboard = value; ApplyChanges(); } }
        public bool EnableMouse { get => settingService.Settings.EnableMouse; set { settingService.Settings.EnableMouse = value; ApplyChanges(); } }
        public bool DragByTitle { get => settingService.Settings.DragByTitle; set { settingService.Settings.DragByTitle = value; ApplyChanges(); } }
        public MouseButton MouseButton { get => settingService.Settings.MouseButton; set { settingService.Settings.MouseButton = value; ApplyChanges(); } }
        public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }
        public int MouseDragDelay { get => settingService.Settings.MouseDragDelay; set { settingService.Settings.MouseDragDelay = value; ApplyChanges(); } }
        public bool EnableHoldKey { get { enableHoldKey = settingService.Settings.EnableHoldKey; return enableHoldKey; } set { settingService.Settings.EnableHoldKey = value; SetProperty(ref enableHoldKey, value); ApplyChanges(); } }
        public HoldKey HoldKey { get => settingService.Settings.HoldKey; set { settingService.Settings.HoldKey = value; ApplyChanges(); } }
        public ObservableCollection<HoldKey> HoldKeys { get => holdKeys; set => SetProperty(ref holdKeys, value); }
        public Resource<HoldKeyBehaviour> HoldKeyBehaviour { get => holdKeyBehaviours.FirstOrDefault(i => i.Key == settingService.Settings.HoldKeyBehaviour); set { settingService.Settings.HoldKeyBehaviour = value.Key; ApplyChanges(); } }
        public ObservableCollection<Resource<HoldKeyBehaviour>> HoldKeyBehaviours { get => holdKeyBehaviours; set => SetProperty(ref holdKeyBehaviours, value); }
        public bool DisableForFullscreen { get => settingService.Settings.DisableForFullscreen; set { settingService.Settings.DisableForFullscreen = value; ApplyChanges(); } }
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

            HoldKeyBehaviours = new ObservableCollection<Resource<HoldKeyBehaviour>>
            {
                new Resource<HoldKeyBehaviour>(Library.Entities.HoldKeyBehaviour.HoldToEnable, "Hold key to enable snapping"),
                new Resource<HoldKeyBehaviour>(Library.Entities.HoldKeyBehaviour.HoldToDisable, "Hold key to disable snapping")
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