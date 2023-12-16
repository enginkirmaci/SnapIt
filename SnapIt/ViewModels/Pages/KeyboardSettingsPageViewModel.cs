using System.Windows;
using Prism.Commands;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Mvvm;
using SnapIt.Services.Contracts;

namespace SnapIt.ViewModels.Pages;

public class KeyboardSettingsPageViewModel : ViewModelBase
{
    private readonly ISnapManager snapManager;
    private readonly ISettingService settingService;

    private bool canApplyChanges = true;
    private string moveUpShortcut;
    private string moveDownShortcut;
    private string moveLeftShortcut;
    private string moveRightShortcut;

    public bool EnableKeyboard
    { get => settingService.Settings.EnableKeyboard; set { settingService.Settings.EnableKeyboard = value; ApplyChanges(); } }

    public string MoveUpShortcut
    { get { moveUpShortcut = settingService.Settings.MoveUpShortcut; return moveUpShortcut; } set { settingService.Settings.MoveUpShortcut = value; SetProperty(ref moveUpShortcut, value); ApplyChanges(); } }

    public string MoveDownShortcut
    { get { moveDownShortcut = settingService.Settings.MoveDownShortcut; return moveDownShortcut; } set { settingService.Settings.MoveDownShortcut = value; SetProperty(ref moveDownShortcut, value); ApplyChanges(); } }

    public string MoveLeftShortcut
    { get { moveLeftShortcut = settingService.Settings.MoveLeftShortcut; return moveLeftShortcut; } set { settingService.Settings.MoveLeftShortcut = value; SetProperty(ref moveLeftShortcut, value); ApplyChanges(); } }

    public string MoveRightShortcut
    { get { moveRightShortcut = settingService.Settings.MoveRightShortcut; return moveRightShortcut; } set { settingService.Settings.MoveRightShortcut = value; SetProperty(ref moveRightShortcut, value); ApplyChanges(); } }

    public string CycleLayoutsShortcut
    { get => settingService.Settings.CycleLayoutsShortcut; set { settingService.Settings.CycleLayoutsShortcut = value; ApplyChanges(); } }

    public string StartStopShortcut
    { get => settingService.Settings.StartStopShortcut; set { settingService.Settings.StartStopShortcut = value; ApplyChanges(); } }

    public DelegateCommand OverrideDefaultSnapCommand { get; set; }

    public KeyboardSettingsPageViewModel(
        ISnapManager snapManager,
        ISettingService settingService)
    {
        this.snapManager = snapManager;
        this.settingService = settingService;

        OverrideDefaultSnapCommand = new DelegateCommand(() =>
        {
            canApplyChanges = false;

            MoveUpShortcut = "Win + Up";
            MoveDownShortcut = "Win + Down";
            MoveLeftShortcut = "Win + Left";
            MoveRightShortcut = "Win + Right";

            canApplyChanges = true;
            ApplyChanges();
        });
    }

    private void ApplyChanges()
    {
        if (canApplyChanges && !Dev.IsActive)
        {
            snapManager.Dispose();
            snapManager.InitializeAsync();
        }
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        await settingService.InitializeAsync();
    }
}