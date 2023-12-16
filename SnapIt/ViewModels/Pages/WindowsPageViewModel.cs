using System.Windows;
using Prism.Commands;
using SnapIt.Application.Contracts;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Mvvm;
using SnapIt.Services.Contracts;
using SnapIt.Views.Dialogs;
using Wpf.Ui;

namespace SnapIt.ViewModels.Pages;

public class WindowsPageViewModel : ViewModelBase
{
    private readonly ISnapManager snapManager;
    private readonly ISettingService settingService;
    private readonly IWinApiService winApiService;
    private readonly IContentDialogService contentDialogService;
    private bool disableForModal;
    private ExcludedApplication selectedExcludedApplication;
    private ObservableCollection<ExcludedApplication> excludedApplications;
    private bool isRunningApplicationsDialogOpen;
    private bool isExcludeApplicationDialogOpen;
    private ObservableCollection<MatchRule> matchRules;

    public bool DisableForFullscreen
    { get => settingService.Settings.DisableForFullscreen; set { settingService.Settings.DisableForFullscreen = value; ApplyChanges(); } }

    public bool DisableForModal
    { get { disableForModal = settingService.Settings.DisableForModal; return disableForModal; } set { settingService.Settings.DisableForModal = value; SetProperty(ref disableForModal, value); ApplyChanges(); } }

    public ObservableCollection<ExcludedApplication> ExcludedApplications { get => excludedApplications; set => SetProperty(ref excludedApplications, value); }
    public ExcludedApplication SelectedExcludedApplication { get => selectedExcludedApplication; set => SetProperty(ref selectedExcludedApplication, value); }
    public bool IsExcludeApplicationDialogOpen { get => isExcludeApplicationDialogOpen; set => SetProperty(ref isExcludeApplicationDialogOpen, value); }
    public bool IsRunningApplicationsDialogOpen { get => isRunningApplicationsDialogOpen; set => SetProperty(ref isRunningApplicationsDialogOpen, value); }
    public ObservableCollection<MatchRule> MatchRules { get => matchRules; set => SetProperty(ref matchRules, value); }

    //public DelegateCommand<RoutedEventArgs> LoadedCommand { get; private set; }
    public DelegateCommand ExcludeWindowsModalCommand { get; private set; }

    public DelegateCommand<ExcludedApplication> OpenExcludeApplicationCommand { get; private set; }
    public DelegateCommand<ExcludedApplication> RemoveExcludedApplicationCommand { get; private set; }

    public WindowsPageViewModel(
        ISnapManager snapManager,
        ISettingService settingService,
        IWinApiService winApiService,
        IContentDialogService contentDialogService)
    {
        this.snapManager = snapManager;
        this.settingService = settingService;
        this.winApiService = winApiService;
        this.contentDialogService = contentDialogService;

        ExcludeWindowsModalCommand = new DelegateCommand(() =>
        {
            if (!ExcludedApplications.Any(e => e.Keyword == "Action center"))
            {
                ExcludedApplications.Add(new ExcludedApplication
                {
                    Keyword = "Action center",
                    MatchRule = MatchRule.Contains,
                    Mouse = true,
                    Keyboard = true
                });
            }

            if (!ExcludedApplications.Any(e => e.Keyword == "Action center"))
            {
                ExcludedApplications.Add(new ExcludedApplication
                {
                    Keyword = "Action center",
                    MatchRule = MatchRule.Contains,
                    Mouse = true,
                    Keyboard = true
                });
            }

            if (!ExcludedApplications.Any(e => e.Keyword == "Start"))
            {
                ExcludedApplications.Add(new ExcludedApplication
                {
                    Keyword = "Start",
                    MatchRule = MatchRule.Contains,
                    Mouse = true,
                    Keyboard = true
                });
            }
            if (!ExcludedApplications.Any(e => e.Keyword == "New notification"))
            {
                ExcludedApplications.Add(new ExcludedApplication
                {
                    Keyword = "New notification",
                    MatchRule = MatchRule.Contains,
                    Mouse = true,
                    Keyboard = true
                });
            }

            settingService.SaveExcludedApps(ExcludedApplications.ToList());
            ApplyChanges();
        });

        OpenExcludeApplicationCommand = new DelegateCommand<ExcludedApplication>(async (selected) =>
        {
            var excludeApplicationDialog = new ExcludeApplicationDialog(contentDialogService.GetContentPresenter())
            {
                IsPrimaryButtonEnabled = true,
                PrimaryButtonText = "Save"
            };

            if (selected != null)
            {
                excludeApplicationDialog.ViewModel.SelectedExcludedApplication = selected;
            }

            var result = await excludeApplicationDialog.ShowAsync();

            if (result == Wpf.Ui.Controls.ContentDialogResult.Primary)
            {
                if (excludeApplicationDialog.ViewModel.SelectedExcludedApplication != null &&
                    !ExcludedApplications.Contains(excludeApplicationDialog.ViewModel.SelectedExcludedApplication))
                {
                    ExcludedApplications.Add(excludeApplicationDialog.ViewModel.SelectedExcludedApplication);
                }
                settingService.SaveExcludedApps(ExcludedApplications.ToList());
                ApplyChanges();
            }
        });

        //ExcludeApplicationDialogClosingCommand = new DelegateCommand<object>((isSave) =>
        //{
        //    IsExcludeApplicationDialogOpen = false;

        //    if ((bool)isSave)
        //    {
        //        if (!ExcludedApplications.Contains(SelectedExcludedApplication))
        //        {
        //            ExcludedApplications.Add(SelectedExcludedApplication);
        //        }
        //        else
        //        {
        //        }

        //        settingService.SaveExcludedApps(ExcludedApplications.ToList());
        //        ApplyChanges();
        //    }
        //});

        RemoveExcludedApplicationCommand = new DelegateCommand<ExcludedApplication>((selected) =>
        {
            ExcludedApplications.Remove(selected);

            settingService.SaveExcludedApps(ExcludedApplications.ToList());
            ApplyChanges();
        });
    }

    public override async Task InitializeAsync(RoutedEventArgs args)
    {
        //await snapManager.InitializeAsync();
        await settingService.InitializeAsync();
        await winApiService.InitializeAsync();

        ExcludedApplications = new ObservableCollection<ExcludedApplication>(settingService.ExcludedApplicationSettings.Applications);
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