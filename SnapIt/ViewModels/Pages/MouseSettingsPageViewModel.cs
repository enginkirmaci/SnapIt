using System.Windows;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Mvvm;
using SnapIt.Services.Contracts;

namespace SnapIt.ViewModels.Pages;

public class MouseSettingsPageViewModel : ViewModelBase
{
    private readonly ISnapManager snapManager;
    private readonly ISettingService settingService;

    private ObservableCollection<MouseButton> mouseButtons;
    private ObservableCollection<HoldKey> holdKeys;
    private ObservableCollection<Resource<HoldKeyBehaviour>> holdKeyBehaviours;
    private bool enableHoldKey;
    private object mouseDragDelay = new object();

    public bool EnableMouse
    { get => settingService.Settings.EnableMouse; set { settingService.Settings.EnableMouse = value; ApplyChanges(); } }

    public bool DragByTitle
    { get => settingService.Settings.DragByTitle; set { settingService.Settings.DragByTitle = value; ApplyChanges(); } }

    public MouseButton MouseButton
    { get => settingService.Settings.MouseButton; set { settingService.Settings.MouseButton = value; ApplyChanges(); } }

    public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }

    public int MouseDragDelay
    {
        get => settingService.Settings.MouseDragDelay;
        set
        {
            settingService.Settings.MouseDragDelay = value;

            ApplyChanges();
        }
    }

    public bool EnableHoldKey
    { get { enableHoldKey = settingService.Settings.EnableHoldKey; return enableHoldKey; } set { settingService.Settings.EnableHoldKey = value; SetProperty(ref enableHoldKey, value); ApplyChanges(); } }

    public HoldKey HoldKey
    { get => settingService.Settings.HoldKey; set { settingService.Settings.HoldKey = value; ApplyChanges(); } }

    public ObservableCollection<HoldKey> HoldKeys { get => holdKeys; set => SetProperty(ref holdKeys, value); }

    public Resource<HoldKeyBehaviour> HoldKeyBehaviour
    { get => holdKeyBehaviours.FirstOrDefault(i => i.Key == settingService.Settings.HoldKeyBehaviour); set { settingService.Settings.HoldKeyBehaviour = value.Key; ApplyChanges(); } }

    public ObservableCollection<Resource<HoldKeyBehaviour>> HoldKeyBehaviours { get => holdKeyBehaviours; set => SetProperty(ref holdKeyBehaviours, value); }

    public MouseSettingsPageViewModel(
        ISnapManager snapManager,
        ISettingService settingService)
    {
        this.snapManager = snapManager;
        this.settingService = settingService;

        MouseButtons =
        [
            MouseButton.Left,
            MouseButton.Middle,
            MouseButton.Right
        ];

        HoldKeys = [
            HoldKey.Control,
            HoldKey.Alt,
            HoldKey.Shift,
            HoldKey.Win
        ];

        HoldKeyBehaviours =
        [
            new Resource<HoldKeyBehaviour>(SnapIt.Common.Entities.HoldKeyBehaviour.HoldToEnable, "Hold key to enable snapping"),
            new Resource<HoldKeyBehaviour>(SnapIt.Common.Entities.HoldKeyBehaviour.HoldToDisable, "Hold key to disable snapping")
        ];
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        await settingService.InitializeAsync();
    }

    private void ApplyChanges()
    {
        if (!Dev.IsActive)
        {
            snapManager.Dispose();
            snapManager.InitializeAsync();
        }
    }
}